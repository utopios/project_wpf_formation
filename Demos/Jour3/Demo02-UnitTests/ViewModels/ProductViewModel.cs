using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TestsDemo.ViewModels;

/// <summary>
/// Interface pour le service de produits (permet le mocking)
/// </summary>
public interface IProductService
{
    Task<IEnumerable<Product>> GetProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task SaveProductAsync(Product product);
    Task DeleteProductAsync(int id);
}

/// <summary>
/// Modèle de produit
/// </summary>
public record Product(int Id, string Name, decimal Price, int Stock);

/// <summary>
/// ViewModel testable avec injection de dépendances
/// </summary>
public partial class ProductViewModel : ObservableObject
{
    private readonly IProductService _productService;

    public ProductViewModel(IProductService productService)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsValid))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _name = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsValid))]
    [NotifyPropertyChangedFor(nameof(FormattedPrice))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private decimal _price;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsValid))]
    [NotifyPropertyChangedFor(nameof(StockStatus))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private int _stock;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    #region Propriétés calculées (à tester)

    public bool IsValid =>
        !string.IsNullOrWhiteSpace(Name) &&
        Price > 0 &&
        Stock >= 0;

    public string FormattedPrice => Price.ToString("C");

    public string StockStatus => Stock switch
    {
        0 => "Épuisé",
        <= 5 => "Stock faible",
        <= 20 => "Stock normal",
        _ => "Stock élevé"
    };

    #endregion

    #region Commandes (à tester)

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        if (!IsValid)
        {
            ErrorMessage = "Données invalides";
            return;
        }

        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var product = new Product(0, Name, Price, Stock);
            await _productService.SaveProductAsync(product);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanSave() => IsValid && !IsLoading;

    [RelayCommand]
    private void Reset()
    {
        Name = string.Empty;
        Price = 0;
        Stock = 0;
        ErrorMessage = null;
    }

    [RelayCommand]
    private void IncrementStock()
    {
        Stock++;
    }

    [RelayCommand(CanExecute = nameof(CanDecrementStock))]
    private void DecrementStock()
    {
        if (Stock > 0)
            Stock--;
    }

    private bool CanDecrementStock() => Stock > 0;

    #endregion
}
