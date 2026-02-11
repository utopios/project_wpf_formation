namespace GestionStock;

/// <summary>
/// Point d'entrée de l'application
/// </summary>
public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        // Initialiser la base de données
        InitializeDatabase();

        // Afficher la fenêtre principale
        var mainWindow = Services.GetRequiredService<Views.MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=gestionstock.db"));

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<ISupplierRepository, SupplierRepository>();

        // Services
        services.AddTransient<IStockService, StockService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IDialogService, DialogService>();

        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ProductListViewModel>();
        services.AddTransient<ProductDetailViewModel>();
        services.AddTransient<ProductEditViewModel>();
        services.AddTransient<CategoryListViewModel>();
        services.AddTransient<SupplierListViewModel>();
        services.AddTransient<SupplierEditViewModel>();
        services.AddTransient<StockMovementViewModel>();
        services.AddTransient<SettingsViewModel>();

        // Views
        services.AddTransient<Views.MainWindow>();
    }

    private void InitializeDatabase()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
    }
}
