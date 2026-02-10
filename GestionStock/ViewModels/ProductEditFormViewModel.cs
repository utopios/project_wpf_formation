using GestionStock.Models;
using GestionStock.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GestionStock.ViewModels;

/// <summary>
/// ViewModel pour le formulaire d'édition de produit (MVVM sans toolkit).
/// Toute la logique métier est ici, la vue ne fait que de l'affichage via Bindings.
/// </summary>
public class ProductEditFormViewModel : BindableBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly Product? _product;
    private readonly bool _isNewProduct;

    // ── Événement pour demander la fermeture de la fenêtre ──
    // Le ViewModel ne connaît pas la Window, il demande via cet événement.
    public event Action<bool>? CloseRequested;

    // ── Commandes ──
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }

    // ── Propriétés avec notification ──

    private string _title = "Nouveau produit";
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string _code = string.Empty;
    public string Code
    {
        get => _code;
        set { SetProperty(ref _code, value); ClearError(nameof(CodeError)); }
    }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set { SetProperty(ref _name, value); ClearError(nameof(NameError)); }
    }

    private string? _description;
    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private decimal _unitPrice;
    public decimal UnitPrice
    {
        get => _unitPrice;
        set { SetProperty(ref _unitPrice, value); ClearError(nameof(PriceError)); }
    }

    private int _quantityInStock;
    public int QuantityInStock
    {
        get => _quantityInStock;
        set { SetProperty(ref _quantityInStock, value); ClearError(nameof(QuantityError)); }
    }

    private int _minimumStock = 10;
    public int MinimumStock
    {
        get => _minimumStock;
        set => SetProperty(ref _minimumStock, value);
    }

    private Category? _selectedCategory;
    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set { SetProperty(ref _selectedCategory, value); ClearError(nameof(CategoryError)); }
    }

    private Supplier? _selectedSupplier;
    public Supplier? SelectedSupplier
    {
        get => _selectedSupplier;
        set => SetProperty(ref _selectedSupplier, value);
    }

    private bool _isActive = true;
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    private IEnumerable<Category> _categories = [];
    public IEnumerable<Category> Categories
    {
        get => _categories;
        set => SetProperty(ref _categories, value);
    }

    private IEnumerable<Supplier> _suppliers = [];
    public IEnumerable<Supplier> Suppliers
    {
        get => _suppliers;
        set => SetProperty(ref _suppliers, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    // ── Propriétés d'erreur de validation ──
    // null = pas d'erreur, non-null = message affiché

    private string? _codeError;
    public string? CodeError
    {
        get => _codeError;
        set => SetProperty(ref _codeError, value);
    }

    private string? _nameError;
    public string? NameError
    {
        get => _nameError;
        set => SetProperty(ref _nameError, value);
    }

    private string? _categoryError;
    public string? CategoryError
    {
        get => _categoryError;
        set => SetProperty(ref _categoryError, value);
    }

    private string? _priceError;
    public string? PriceError
    {
        get => _priceError;
        set => SetProperty(ref _priceError, value);
    }

    private string? _quantityError;
    public string? QuantityError
    {
        get => _quantityError;
        set => SetProperty(ref _quantityError, value);
    }

    // ── Constructeur ──

    public ProductEditFormViewModel(Product? product = null)
    {
        // Récupérer les repositories via App.Services (Option 1)
        _productRepository = App.Services.GetRequiredService<IProductRepository>();
        _categoryRepository = App.Services.GetRequiredService<ICategoryRepository>();
        _supplierRepository = App.Services.GetRequiredService<ISupplierRepository>();

        _product = product;
        _isNewProduct = product == null;

        Title = _isNewProduct ? "Nouveau produit" : "Modifier le produit";

        // Initialiser les commandes
        SaveCommand = new DelegateCommand(_ => _ = SaveAsync(), _ => !IsLoading);
        CancelCommand = new DelegateCommand(_ => Cancel());

        // Charger les données du produit si modification
        if (!_isNewProduct)
        {
            LoadProductData();
        }

        // Charger les listes déroulantes
        _ = LoadDataAsync();
    }

    // ── Chargement ──

    private async Task LoadDataAsync()
    {
        IsLoading = true;

        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            Categories = categories;

            var suppliers = await _supplierRepository.GetActiveAsync();
            var suppliersList = new List<Supplier> { new Supplier { Id = 0, Name = "Aucun" } };
            suppliersList.AddRange(suppliers);
            Suppliers = suppliersList;

            // Sélectionner les valeurs du produit après le chargement des listes
            if (_product != null)
            {
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == _product.CategoryId);
                SelectedSupplier = _product.SupplierId.HasValue
                    ? Suppliers.FirstOrDefault(s => s.Id == _product.SupplierId.Value)
                    : Suppliers.First(); // "Aucun"
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erreur lors du chargement des données : {ex.Message}",
                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void LoadProductData()
    {
        if (_product == null) return;

        Code = _product.Code;
        Name = _product.Name;
        Description = _product.Description;
        UnitPrice = _product.UnitPrice;
        QuantityInStock = _product.QuantityInStock;
        MinimumStock = _product.MinimumStock;
        IsActive = _product.IsActive;
    }

    // ── Sauvegarde ──

    private async Task SaveAsync()
    {
        if (!Validate())
            return;

        IsLoading = true;

        try
        {
            if (_isNewProduct)
            {
                var newProduct = new Product
                {
                    Code = Code.Trim(),
                    Name = Name.Trim(),
                    Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                    CategoryId = SelectedCategory!.Id,
                    SupplierId = SelectedSupplier?.Id is > 0 ? SelectedSupplier.Id : null,
                    UnitPrice = UnitPrice,
                    QuantityInStock = QuantityInStock,
                    MinimumStock = MinimumStock,
                    IsActive = IsActive
                };

                await _productRepository.AddAsync(newProduct);
                //Pas correct, utilisation d'event ou propriété avec Binding avec la Vue
                MessageBox.Show("Le produit a été créé avec succès.", "Succès",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                _product!.Code = Code.Trim();
                _product.Name = Name.Trim();
                _product.Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim();
                _product.CategoryId = SelectedCategory!.Id;
                _product.SupplierId = SelectedSupplier?.Id is > 0 ? SelectedSupplier.Id : null;
                _product.UnitPrice = UnitPrice;
                _product.QuantityInStock = QuantityInStock;
                _product.MinimumStock = MinimumStock;
                _product.IsActive = IsActive;

                await _productRepository.UpdateAsync(_product);
                MessageBox.Show("Le produit a été mis à jour avec succès.", "Succès",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            CloseRequested?.Invoke(true);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erreur lors de l'enregistrement : {ex.Message}",
                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    // ── Annulation ──

    private void Cancel()
    {
        CloseRequested?.Invoke(false);
    }

    // ── Validation ──

    private bool Validate()
    {
        bool isValid = true;

        // Réinitialiser les erreurs
        CodeError = null;
        NameError = null;
        CategoryError = null;
        PriceError = null;
        QuantityError = null;

        if (string.IsNullOrWhiteSpace(Code) || Code.Length < 3)
        {
            CodeError = "Le code doit contenir au moins 3 caractères";
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(Name) || Name.Length < 2)
        {
            NameError = "Le nom doit contenir au moins 2 caractères";
            isValid = false;
        }

        if (SelectedCategory == null)
        {
            CategoryError = "Veuillez sélectionner une catégorie";
            isValid = false;
        }

        if (UnitPrice < 0)
        {
            PriceError = "Le prix doit être un nombre positif";
            isValid = false;
        }

        if (QuantityInStock < 0)
        {
            QuantityError = "La quantité doit être un nombre positif";
            isValid = false;
        }

        return isValid;
    }

    private void ClearError(string errorPropertyName)
    {
        switch (errorPropertyName)
        {
            case nameof(CodeError): CodeError = null; break;
            case nameof(NameError): NameError = null; break;
            case nameof(CategoryError): CategoryError = null; break;
            case nameof(PriceError): PriceError = null; break;
            case nameof(QuantityError): QuantityError = null; break;
        }
    }
}
