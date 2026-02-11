using GestionStock.Models;

namespace GestionStock.ViewModels;

/// <summary>
/// ViewModel pour l'affichage des détails d'un produit (MVVM avec CommunityToolkit).
/// Toute la logique d'affichage est ici, la vue ne fait que du Binding.
/// </summary>
public partial class ProductInfoViewModel : ObservableObject
{
    private readonly Product _product;

    // ── Événement pour demander la fermeture de la fenêtre ──
    public event Action<bool>? CloseRequested;

    // ── Propriétés observables ──

    [ObservableProperty]
    private string _productName = string.Empty;

    [ObservableProperty]
    private string _productCode = string.Empty;

    [ObservableProperty]
    private string _categoryName = string.Empty;

    [ObservableProperty]
    private string _supplierName = string.Empty;

    [ObservableProperty]
    private string _priceDisplay = string.Empty;

    // Partie 4 (Bonus) : NotifyPropertyChangedFor pour les propriétés calculées
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StockStatusText))]
    [NotifyPropertyChangedFor(nameof(StockStatusColor))]
    private int _quantityInStock;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StockStatusText))]
    [NotifyPropertyChangedFor(nameof(StockStatusColor))]
    private int _minimumStock;

    [ObservableProperty]
    private string _totalValueDisplay = string.Empty;

    [ObservableProperty]
    private string _activeStatus = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _createdAtDisplay = string.Empty;

    [ObservableProperty]
    private string _updatedAtDisplay = string.Empty;

    // ── Propriétés calculées (Bonus Partie 4) ──
    // Pas de [ObservableProperty] : ce sont des getters purs,
    // notifiés automatiquement quand QuantityInStock ou MinimumStock changent.

    public string StockStatusText => QuantityInStock == 0
        ? "Rupture de stock"
        : QuantityInStock <= MinimumStock
            ? "Stock faible"
            : "En stock";

    public string StockStatusColor => QuantityInStock == 0
        ? "#F44336"
        : QuantityInStock <= MinimumStock
            ? "#FF9800"
            : "#4CAF50";

    // ── Constructeur ──

    public ProductInfoViewModel(Product product)
    {
        _product = product ?? throw new ArgumentNullException(nameof(product));
        LoadFromProduct();
    }

    /// <summary>
    /// Remplit toutes les propriétés à partir du modèle Product.
    /// </summary>
    private void LoadFromProduct()
    {
        ProductName = _product.Name;
        ProductCode = $"Code: {_product.Code}";
        CategoryName = _product.Category?.Name ?? "Non définie";
        SupplierName = _product.Supplier?.Name ?? "Aucun";
        PriceDisplay = $"{_product.UnitPrice:C}";
        QuantityInStock = _product.QuantityInStock;
        MinimumStock = _product.MinimumStock;
        TotalValueDisplay = $"{_product.TotalValue:C}";
        ActiveStatus = _product.IsActive ? "Actif" : "Inactif";
        Description = string.IsNullOrWhiteSpace(_product.Description)
            ? "Aucune description"
            : _product.Description;
        CreatedAtDisplay = _product.CreatedAt.ToString("dd/MM/yyyy HH:mm");
        UpdatedAtDisplay = _product.UpdatedAt.HasValue
            ? _product.UpdatedAt.Value.ToString("dd/MM/yyyy HH:mm")
            : "Jamais modifié";
    }

    // ── Commandes ──

    [RelayCommand]
    private void Edit()
    {
        var editWindow = new Views.ProductEditView(_product);
        if (editWindow.ShowDialog() == true)
        {
            // Recharger les données après modification
            LoadFromProduct();
        }
    }

    [RelayCommand]
    private void CloseWindow()
    {
        CloseRequested?.Invoke(false);
    }
}
