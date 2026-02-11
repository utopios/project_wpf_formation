using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ConfigDemo.Settings;
using ConfigDemo.ViewModels;

namespace ConfigDemo;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;
    public static IConfiguration Configuration { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 1. Construire la configuration
        Configuration = BuildConfiguration();

        // 2. Configurer les services
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        // 3. Afficher la fenêtre principale
        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    /// <summary>
    /// Construction de la configuration multi-sources
    /// </summary>
    private static IConfiguration BuildConfiguration()
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

        return new ConfigurationBuilder()
            // Répertoire de base
            .SetBasePath(AppContext.BaseDirectory)
            // Configuration par défaut
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            // Configuration spécifique à l'environnement
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            // Variables d'environnement (override)
            .AddEnvironmentVariables()
            .Build();
    }

    /// <summary>
    /// Configuration des services avec Options pattern
    /// </summary>
    private void ConfigureServices(IServiceCollection services)
    {
        // Enregistrer IConfiguration
        services.AddSingleton(Configuration);

        // Options pattern - fortement typé
        services.Configure<AppSettings>(Configuration.GetSection(AppSettings.SectionName));
        services.Configure<ApiSettings>(Configuration.GetSection(ApiSettings.SectionName));
        services.Configure<FeatureFlags>(Configuration.GetSection(FeatureFlags.SectionName));

        // ViewModels
        services.AddTransient<MainViewModel>();

        // Views
        services.AddTransient<MainWindow>();
    }
}
