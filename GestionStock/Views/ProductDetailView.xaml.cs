using System.Windows;
using System.Windows.Media;
using GestionStock.Models;

namespace GestionStock.Views;

/// <summary>
/// Fenêtre de détails d'un produit (sans MVVM)
/// </summary>
public partial class ProductDetailView : Window
{
    private readonly Product _product;
    private readonly IProductRepository _repository = App.Services.GetRequiredService<IProductRepository>();
    public ProductDetailView(Product product)
    {
        InitializeComponent();
        _product = product ?? throw new ArgumentNullException(nameof(product));
        LoadProductDetails();
    }

    /// <summary>
    /// Charge les détails du produit dans les contrôles
    /// </summary>
    private void LoadProductDetails()
    {
        // Informations principales
        ProductNameText.Text = _product.Name;
        ProductCodeText.Text = $"Code: {_product.Code}";

        // Catégorie et Fournisseur
        CategoryText.Text = _product.Category?.Name ?? "Non définie";
        SupplierText.Text = _product.Supplier?.Name ?? "Aucun";

        // Prix et quantités
        PriceText.Text = $"{_product.UnitPrice:C}";
        QuantityText.Text = _product.QuantityInStock.ToString();
        MinStockText.Text = _product.MinimumStock.ToString();
        TotalValueText.Text = $"{_product.TotalValue:C}";

        // Statut actif
        ActiveText.Text = _product.IsActive ? "Actif" : "Inactif";

        // Description
        DescriptionText.Text = string.IsNullOrWhiteSpace(_product.Description)
            ? "Aucune description"
            : _product.Description;

        // Dates
        CreatedAtText.Text = _product.CreatedAt.ToString("dd/MM/yyyy HH:mm");
        UpdatedAtText.Text = _product.UpdatedAt.HasValue
            ? _product.UpdatedAt.Value.ToString("dd/MM/yyyy HH:mm")
            : "Jamais modifié";

        // Badge de statut de stock
        UpdateStatusBadge();
    }

    /// <summary>
    /// Met à jour le badge de statut du stock
    /// </summary>
    private void UpdateStatusBadge()
    {
        if (_product.IsOutOfStock)
        {
            StatusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F44336"));
            StatusText.Text = "Rupture de stock";
        }
        else if (_product.IsLowStock)
        {
            StatusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9800"));
            StatusText.Text = "Stock faible";
        }
        else
        {
            StatusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
            StatusText.Text = "En stock";
        }
    }

    /// <summary>
    /// Gestionnaire pour le bouton Modifier
    /// </summary>
    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        var editWindow = new ProductEditView(_product);
        if (editWindow.ShowDialog() == true)
        {
            // Recharger les données du produit après modification
            MessageBox.Show(
                "Produit modifié. Veuillez fermer cette fenêtre et rouvrir les détails pour voir les changements.",
                "Information",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }

    /// <summary>
    /// Gestionnaire pour le bouton Fermer
    /// </summary>
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
