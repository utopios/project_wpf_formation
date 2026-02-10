namespace GestionStock.ViewModels;

/// <summary>
/// ViewModel du tableau de bord
/// </summary>
public partial class DashboardViewModel : NavigableViewModelBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IStockService _stockService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private int _totalProducts;

    [ObservableProperty]
    private int _lowStockCount;

    [ObservableProperty]
    private int _outOfStockCount;

    [ObservableProperty]
    private decimal _totalStockValue;

    [ObservableProperty]
    private ObservableCollection<Product> _lowStockProducts = [];

    [ObservableProperty]
    private ObservableCollection<StockMovement> _recentMovements = [];

    [ObservableProperty]
    private ObservableCollection<CategoryStats> _categoryStats = [];

    public DashboardViewModel(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IStockService stockService,
        INavigationService navigationService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _stockService = stockService;
        _navigationService = navigationService;
    }

    public override void OnNavigatedTo(object? parameter)
    {
        LoadDashboardCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadDashboardAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Charger les statistiques en parallèle
            var productsTask = _productRepository.GetAllAsync();
            var lowStockTask = _productRepository.GetLowStockProductsAsync();
            var outOfStockTask = _productRepository.GetOutOfStockProductsAsync();
            var movementsTask = _stockService.GetRecentMovementsAsync(10);
            var categoriesTask = _categoryRepository.GetWithProductCountAsync();

            await Task.WhenAll(productsTask, lowStockTask, outOfStockTask, movementsTask, categoriesTask);

            var products = await productsTask;
            var lowStock = await lowStockTask;
            var outOfStock = await outOfStockTask;
            var movements = await movementsTask;
            var categories = await categoriesTask;

            // Mettre à jour les compteurs
            TotalProducts = products.Count();
            LowStockCount = lowStock.Count();
            OutOfStockCount = outOfStock.Count();
            TotalStockValue = products.Sum(p => p.TotalValue);

            // Mettre à jour les collections
            LowStockProducts = new ObservableCollection<Product>(lowStock.Take(5));
            RecentMovements = new ObservableCollection<StockMovement>(movements);

            // Statistiques par catégorie
            CategoryStats = new ObservableCollection<CategoryStats>(
                categories.Select(c => new CategoryStats
                {
                    CategoryName = c.Name,
                    ProductCount = c.Products.Count,
                    TotalValue = c.Products.Sum(p => p.TotalValue)
                }));
        });
    }

    [RelayCommand]
    private void ViewProduct(Product product)
    {
        _navigationService.NavigateTo<ProductDetailViewModel, int>(product.Id);
    }

    [RelayCommand]
    private void ViewLowStock()
    {
        _navigationService.NavigateTo<ProductListViewModel, string>("lowstock");
    }

    [RelayCommand]
    private void ViewOutOfStock()
    {
        _navigationService.NavigateTo<ProductListViewModel, string>("outofstock");
    }
}

/// <summary>
/// Statistiques par catégorie pour le dashboard
/// </summary>
public class CategoryStats
{
    public string CategoryName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public decimal TotalValue { get; set; }
}
