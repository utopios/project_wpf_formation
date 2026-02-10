namespace GestionStock.Services;

/// <summary>
/// Interface générique pour les repositories
/// </summary>
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

/// <summary>
/// Repository spécifique pour les produits
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> GetLowStockProductsAsync();
    Task<IEnumerable<Product>> GetOutOfStockProductsAsync();
    Task<IEnumerable<Product>> SearchAsync(string searchTerm);
    Task<Product?> GetByCodeAsync(string code);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}

/// <summary>
/// Repository pour les catégories
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetWithProductCountAsync();
    Task<bool> NameExistsAsync(string name, int? excludeId = null);
}

/// <summary>
/// Repository pour les fournisseurs
/// </summary>
public interface ISupplierRepository : IRepository<Supplier>
{
    Task<IEnumerable<Supplier>> GetActiveAsync();
}

/// <summary>
/// Service pour les mouvements de stock
/// </summary>
public interface IStockService
{
    Task<StockMovement> AddStockAsync(int productId, int quantity, string? reason = null, string? reference = null);
    Task<StockMovement> RemoveStockAsync(int productId, int quantity, string? reason = null, string? reference = null);
    Task<StockMovement> AdjustStockAsync(int productId, int newQuantity, string? reason = null);
    Task<IEnumerable<StockMovement>> GetMovementsForProductAsync(int productId);
    Task<IEnumerable<StockMovement>> GetRecentMovementsAsync(int count = 50);
}

/// <summary>
/// Service de navigation
/// </summary>
public interface INavigationService
{
    object? CurrentViewModel { get; }
    void NavigateTo<TViewModel>() where TViewModel : class;
    void NavigateTo<TViewModel, TParameter>(TParameter parameter) where TViewModel : class;
    void GoBack();
    bool CanGoBack { get; }
    event EventHandler<Type>? Navigated;
}

/// <summary>
/// Service de dialogue
/// </summary>
public interface IDialogService
{
    Task<bool> ConfirmAsync(string title, string message);
    Task AlertAsync(string title, string message);
    Task<string?> PromptAsync(string title, string message, string defaultValue = "");
    void ShowNotification(string message, NotificationType type = NotificationType.Info);
}

/// <summary>
/// Type de notification
/// </summary>
public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error
}

/// <summary>
/// Interface pour les ViewModels qui supportent la navigation
/// </summary>
public interface INavigationAware
{
    void OnNavigatedTo(object? parameter);
    void OnNavigatedFrom();
}

/// <summary>
/// Interface pour les ViewModels de dialogue
/// </summary>
public interface IDialogViewModel<TResult>
{
    TResult? Result { get; }
    bool? DialogResult { get; set; }
    void Initialize(object? parameter);
}
