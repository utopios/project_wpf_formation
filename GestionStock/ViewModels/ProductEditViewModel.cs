namespace GestionStock.ViewModels;

/// <summary>
/// ViewModel pour l'édition/création d'un produit
/// </summary>
public partial class ProductEditViewModel : NavigableViewModelBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    private int? _productId;
    private bool _isNew;

    [ObservableProperty]
    private string _title = "Nouveau produit";

    [ObservableProperty]
    private ObservableCollection<Category> _categories = [];

    [ObservableProperty]
    private ObservableCollection<Supplier> _suppliers = [];

    // Propriétés du formulaire avec validation
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Le code est requis")]
    [MinLength(3, ErrorMessage = "Le code doit contenir au moins 3 caractères")]
    [MaxLength(20, ErrorMessage = "Le code ne peut pas dépasser 20 caractères")]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _code = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Le nom est requis")]
    [MinLength(2, ErrorMessage = "Le nom doit contenir au moins 2 caractères")]
    [MaxLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _name = string.Empty;

    [ObservableProperty]
    [MaxLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
    private string? _description;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être positif")]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private decimal _unitPrice;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(0, int.MaxValue, ErrorMessage = "La quantité doit être positive")]
    private int _quantityInStock;

    [ObservableProperty]
    [Range(0, int.MaxValue, ErrorMessage = "Le stock minimum doit être positif")]
    private int _minimumStock = 10;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "La catégorie est requise")]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private Category? _selectedCategory;

    [ObservableProperty]
    private Supplier? _selectedSupplier;

    public ProductEditViewModel(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ISupplierRepository supplierRepository,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _supplierRepository = supplierRepository;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    public override void OnNavigatedTo(object? parameter)
    {
        if (parameter is int productId)
        {
            _productId = productId;
            _isNew = false;
            Title = "Modifier le produit";
        }
        else
        {
            _isNew = true;
            Title = "Nouveau produit";
        }

        LoadDataCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Charger les listes de référence
            var categoriesTask = _categoryRepository.GetAllAsync();
            var suppliersTask = _supplierRepository.GetActiveAsync();

            await Task.WhenAll(categoriesTask, suppliersTask);

            Categories = new ObservableCollection<Category>(await categoriesTask);
            Suppliers = new ObservableCollection<Supplier>(await suppliersTask);

            // Si édition, charger le produit
            if (_productId.HasValue)
            {
                var product = await _productRepository.GetByIdAsync(_productId.Value);
                if (product != null)
                {
                    Code = product.Code;
                    Name = product.Name;
                    Description = product.Description;
                    UnitPrice = product.UnitPrice;
                    QuantityInStock = product.QuantityInStock;
                    MinimumStock = product.MinimumStock;
                    SelectedCategory = Categories.FirstOrDefault(c => c.Id == product.CategoryId);
                    SelectedSupplier = Suppliers.FirstOrDefault(s => s.Id == product.SupplierId);
                }
            }
        });
    }

    private bool CanSave()
    {
        ValidateAllProperties();
        return !HasErrors &&
               !string.IsNullOrWhiteSpace(Code) &&
               !string.IsNullOrWhiteSpace(Name) &&
               SelectedCategory != null;
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        ValidateAllProperties();
        if (HasErrors) return;

        // Vérifier l'unicité du code
        var codeExists = await _productRepository.CodeExistsAsync(Code, _productId);
        if (codeExists)
        {
            await _dialogService.AlertAsync("Erreur", "Ce code produit existe déjà.");
            return;
        }

        await ExecuteAsync(async () =>
        {
            Product product;

            if (_isNew)
            {
                product = new Product
                {
                    Code = Code,
                    Name = Name,
                    Description = Description,
                    UnitPrice = UnitPrice,
                    QuantityInStock = QuantityInStock,
                    MinimumStock = MinimumStock,
                    CategoryId = SelectedCategory!.Id,
                    SupplierId = SelectedSupplier?.Id
                };

                await _productRepository.AddAsync(product);
                _dialogService.ShowNotification("Produit créé avec succès", NotificationType.Success);
            }
            else
            {
                product = await _productRepository.GetByIdAsync(_productId!.Value)
                    ?? throw new InvalidOperationException("Produit non trouvé");

                product.Code = Code;
                product.Name = Name;
                product.Description = Description;
                product.UnitPrice = UnitPrice;
                product.MinimumStock = MinimumStock;
                product.CategoryId = SelectedCategory!.Id;
                product.SupplierId = SelectedSupplier?.Id;

                await _productRepository.UpdateAsync(product);
                _dialogService.ShowNotification("Produit modifié avec succès", NotificationType.Success);
            }

            _navigationService.GoBack();
        });
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        if (HasChanges())
        {
            var confirm = await _dialogService.ConfirmAsync(
                "Modifications non sauvegardées",
                "Voulez-vous vraiment annuler ? Les modifications seront perdues.");

            if (!confirm) return;
        }

        _navigationService.GoBack();
    }

    private bool HasChanges()
    {
        // Vérifier si le formulaire a été modifié
        return !string.IsNullOrEmpty(Code) || !string.IsNullOrEmpty(Name);
    }
}
