using System.Windows;
using System.Windows.Controls;
using GestionStock.Models;
using GestionStock.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GestionStock.Views;

/// <summary>
/// Fenêtre d'édition de produit (sans MVVM)
/// </summary>
public partial class ProductEditView : Window
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly Product? _product;
    private readonly bool _isNewProduct;

    /// <summary>
    /// Constructeur pour créer un nouveau produit
    /// </summary>
    public ProductEditView()
    {
        InitializeComponent();

        // Utilisation de l'Option 1 : App.Services
        _productRepository = App.Services.GetRequiredService<IProductRepository>();
        _categoryRepository = App.Services.GetRequiredService<ICategoryRepository>();
        _supplierRepository = App.Services.GetRequiredService<ISupplierRepository>();

        _isNewProduct = true;
        TitleText.Text = "Nouveau produit";

        _ = LoadDataAsync();
    }

    /// <summary>
    /// Constructeur pour modifier un produit existant
    /// </summary>
    public ProductEditView(Product product) : this()
    {
        _product = product ?? throw new ArgumentNullException(nameof(product));
        _isNewProduct = false;
        TitleText.Text = "Modifier le produit";

        LoadProductData();
    }

    /// <summary>
    /// Charge les données (catégories et fournisseurs)
    /// </summary>
    private async Task LoadDataAsync()
    {
        ShowLoading(true);

        try
        {
            // Charger les catégories
            var categories = await _categoryRepository.GetAllAsync();
            CategoryComboBox.ItemsSource = categories;

            // Charger les fournisseurs
            var suppliers = await _supplierRepository.GetActiveAsync();

            // Ajouter une option "Aucun" pour le fournisseur
            var suppliersList = new List<Supplier> { new Supplier { Id = 0, Name = "Aucun" } };
            suppliersList.AddRange(suppliers);
            SupplierComboBox.ItemsSource = suppliersList;
            SupplierComboBox.SelectedIndex = 0; // Sélectionner "Aucun" par défaut
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erreur lors du chargement des données : {ex.Message}",
                "Erreur",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            ShowLoading(false);
        }
    }

    /// <summary>
    /// Charge les données du produit dans le formulaire
    /// </summary>
    private void LoadProductData()
    {
        if (_product == null) return;

        CodeTextBox.Text = _product.Code;
        NameTextBox.Text = _product.Name;
        DescriptionTextBox.Text = _product.Description ?? string.Empty;
        PriceTextBox.Text = _product.UnitPrice.ToString("F2");
        QuantityTextBox.Text = _product.QuantityInStock.ToString();
        MinStockTextBox.Text = _product.MinimumStock.ToString();
        IsActiveCheckBox.IsChecked = _product.IsActive;

        // Sélectionner la catégorie
        CategoryComboBox.SelectedValue = _product.CategoryId;

        // Sélectionner le fournisseur
        if (_product.SupplierId.HasValue)
        {
            SupplierComboBox.SelectedValue = _product.SupplierId.Value;
        }
    }

    /// <summary>
    /// Gestionnaire pour le bouton Enregistrer
    /// </summary>
    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateForm())
            return;

        ShowLoading(true);

        try
        {
            if (_isNewProduct)
            {
                await CreateProductAsync();
            }
            else
            {
                await UpdateProductAsync();
            }

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erreur lors de l'enregistrement : {ex.Message}",
                "Erreur",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            ShowLoading(false);
        }
    }

    /// <summary>
    /// Crée un nouveau produit
    /// </summary>
    private async Task CreateProductAsync()
    {
        var newProduct = new Product
        {
            Code = CodeTextBox.Text.Trim(),
            Name = NameTextBox.Text.Trim(),
            Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text)
                ? null
                : DescriptionTextBox.Text.Trim(),
            CategoryId = (int)CategoryComboBox.SelectedValue,
            SupplierId = SupplierComboBox.SelectedValue is int supplierId && supplierId > 0
                ? supplierId
                : null,
            UnitPrice = decimal.Parse(PriceTextBox.Text),
            QuantityInStock = int.Parse(QuantityTextBox.Text),
            MinimumStock = int.Parse(MinStockTextBox.Text),
            IsActive = IsActiveCheckBox.IsChecked ?? true
        };

        await _productRepository.AddAsync(newProduct);

        MessageBox.Show(
            "Le produit a été créé avec succès.",
            "Succès",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    /// <summary>
    /// Met à jour un produit existant
    /// </summary>
    private async Task UpdateProductAsync()
    {
        if (_product == null) return;

        _product.Code = CodeTextBox.Text.Trim();
        _product.Name = NameTextBox.Text.Trim();
        _product.Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text)
            ? null
            : DescriptionTextBox.Text.Trim();
        _product.CategoryId = (int)CategoryComboBox.SelectedValue;
        _product.SupplierId = SupplierComboBox.SelectedValue is int supplierId && supplierId > 0
            ? supplierId
            : null;
        _product.UnitPrice = decimal.Parse(PriceTextBox.Text);
        _product.QuantityInStock = int.Parse(QuantityTextBox.Text);
        _product.MinimumStock = int.Parse(MinStockTextBox.Text);
        _product.IsActive = IsActiveCheckBox.IsChecked ?? true;

        await _productRepository.UpdateAsync(_product);

        MessageBox.Show(
            "Le produit a été mis à jour avec succès.",
            "Succès",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    /// <summary>
    /// Valide le formulaire
    /// </summary>
    private bool ValidateForm()
    {
        bool isValid = true;

        // Réinitialiser les erreurs
        ClearErrors();

        // Code
        if (string.IsNullOrWhiteSpace(CodeTextBox.Text) || CodeTextBox.Text.Length < 3)
        {
            ShowError(CodeErrorText, "Le code doit contenir au moins 3 caractères");
            isValid = false;
        }

        // Nom
        if (string.IsNullOrWhiteSpace(NameTextBox.Text) || NameTextBox.Text.Length < 2)
        {
            ShowError(NameErrorText, "Le nom doit contenir au moins 2 caractères");
            isValid = false;
        }

        // Catégorie
        if (CategoryComboBox.SelectedValue == null)
        {
            ShowError(CategoryErrorText, "Veuillez sélectionner une catégorie");
            isValid = false;
        }

        // Prix
        if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0)
        {
            ShowError(PriceErrorText, "Le prix doit être un nombre positif");
            isValid = false;
        }

        // Quantité
        if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 0)
        {
            ShowError(QuantityErrorText, "La quantité doit être un nombre positif");
            isValid = false;
        }

        // Stock minimum
        if (!int.TryParse(MinStockTextBox.Text, out int minStock) || minStock < 0)
        {
            ShowError(MinStockErrorText, "Le stock minimum doit être un nombre positif");
            isValid = false;
        }

        return isValid;
    }

    /// <summary>
    /// Affiche un message d'erreur
    /// </summary>
    private void ShowError(TextBlock errorTextBlock, string message)
    {
        errorTextBlock.Text = message;
        errorTextBlock.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// Efface tous les messages d'erreur
    /// </summary>
    private void ClearErrors()
    {
        CodeErrorText.Visibility = Visibility.Collapsed;
        NameErrorText.Visibility = Visibility.Collapsed;
        CategoryErrorText.Visibility = Visibility.Collapsed;
        PriceErrorText.Visibility = Visibility.Collapsed;
        QuantityErrorText.Visibility = Visibility.Collapsed;
        MinStockErrorText.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Affiche/masque l'overlay de chargement
    /// </summary>
    private void ShowLoading(bool show)
    {
        LoadingOverlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        SaveButton.IsEnabled = !show;
        CancelButton.IsEnabled = !show;
    }

    /// <summary>
    /// Gestionnaire pour le bouton Annuler
    /// </summary>
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
