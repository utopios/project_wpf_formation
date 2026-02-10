namespace GestionStock.ViewModels;

/// <summary>
/// ViewModel pour la liste des produits
/// </summary>
public partial class ProductListViewModel : NavigableViewModelBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    private string? _filterMode;

    [ObservableProperty]
    private ObservableCollection<Product> _products = [];

    [ObservableProperty]
    private ObservableCollection<Category> _categories = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedProduct))]
    [NotifyCanExecuteChangedFor(nameof(EditProductCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteProductCommand))]
    [NotifyCanExecuteChangedFor(nameof(ViewProductCommand))]
    private Product? _selectedProduct;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilterText))]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private Category? _selectedCategory;

    [ObservableProperty]
    private string _title = "Produits";

    public bool HasSelectedProduct => SelectedProduct != null;

    public string FilterText => string.IsNullOrEmpty(SearchText)
        ? (SelectedCategory != null ? $"Catégorie: {SelectedCategory.Name}" : "Tous les produits")
        : $"Recherche: {SearchText}";

    public ProductListViewModel(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    public override void OnNavigatedTo(object? parameter)
    {
        _filterMode = parameter as string;

        Title = _filterMode switch
        {
            "lowstock" => "Produits - Stock faible",
            "outofstock" => "Produits - Rupture de stock",
            _ => "Produits"
        };

        LoadDataCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Charger les catégories
            var categories = await _categoryRepository.GetAllAsync();
            Categories = new ObservableCollection<Category>(categories);

            // Charger les produits selon le filtre
            IEnumerable<Product> products = _filterMode switch
            {
                "lowstock" => await _productRepository.GetLowStockProductsAsync(),
                "outofstock" => await _productRepository.GetOutOfStockProductsAsync(),
                _ => await _productRepository.GetAllAsync()
            };

            Products = new ObservableCollection<Product>(products);
        });
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = SearchAsync();
    }

    partial void OnSelectedCategoryChanged(Category? value)
    {
        _ = FilterByCategoryAsync();
    }

    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await LoadDataAsync();
            return;
        }

        await ExecuteAsync(async () =>
        {
            var results = await _productRepository.SearchAsync(SearchText);
            Products = new ObservableCollection<Product>(results);
        });
    }

    private async Task FilterByCategoryAsync()
    {
        if (SelectedCategory == null)
        {
            await LoadDataAsync();
            return;
        }

        await ExecuteAsync(async () =>
        {
            var products = await _productRepository.GetByCategoryAsync(SelectedCategory.Id);
            Products = new ObservableCollection<Product>(products);
        });
    }

    [RelayCommand]
    private void AddProduct()
    {
        var editWindow = new Views.ProductEditView();
        if (editWindow.ShowDialog() == true)
        {
            // Recharger la liste après l'ajout
            LoadDataCommand.Execute(null);
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelectedProduct))]
    private void ViewProduct()
    {
        if (SelectedProduct != null)
        {
            // Ouvre la fenêtre de détails sans MVVM
            var detailWindow = new Views.ProductDetailView(SelectedProduct);
            detailWindow.ShowDialog();
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelectedProduct))]
    private void EditProduct()
    {
        if (SelectedProduct != null)
        {
            var editWindow = new Views.ProductEditView(SelectedProduct);
            if (editWindow.ShowDialog() == true)
            {
                // Recharger la liste après la modification
                LoadDataCommand.Execute(null);
            }
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelectedProduct))]
    private async Task DeleteProductAsync()
    {
        if (SelectedProduct == null) return;

        var confirmed = await _dialogService.ConfirmAsync(
            "Confirmation",
            $"Voulez-vous vraiment supprimer le produit '{SelectedProduct.Name}' ?");

        if (confirmed)
        {
            await ExecuteAsync(async () =>
            {
                await _productRepository.DeleteAsync(SelectedProduct.Id);
                Products.Remove(SelectedProduct);
                SelectedProduct = null;
                _dialogService.ShowNotification("Produit supprimé avec succès", NotificationType.Success);
            });
        }
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SearchText = string.Empty;
        SelectedCategory = null;
        _filterMode = null;
        Title = "Produits";
        LoadDataCommand.Execute(null);
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadDataCommand.Execute(null);
    }
}
