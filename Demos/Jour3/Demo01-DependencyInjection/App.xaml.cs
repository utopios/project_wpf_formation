using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using DIDemo.Services;
using DIDemo.ViewModels;
using DIDemo.Infrastructures;

namespace DIDemo;

public partial class App : Application
{
    /// <summary>
    /// Conteneur de services - point d'accès global
    /// Dans une architecture plus complexe, éviter l'accès statique
    /// </summary>
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 1. Créer le conteneur de services
        var services = new ServiceCollection();

        // 2. Enregistrer les services
        ConfigureServices(services);

        // 3. Construire le provider
        Services = services.BuildServiceProvider();

        // 4. Créer et afficher la fenêtre principale via DI
        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    /// <summary>
    /// Configuration des services - le cœur de la DI
    /// </summary>
    private static void ConfigureServices(IServiceCollection services)
    {
        // Services avec différents lifetimes
        // Singleton: une seule instance pour toute l'application
        //services.AddSingleton<IMessageService, MessageService>();

        //// Singleton: le DataService garde son état entre les vues
        //services.AddSingleton<IDataService, DataService>();

        services.AddCustomServices();

        //// Transient: nouvelle instance à chaque résolution
        services.AddTransient<INavigationService, NavigationService>();

        // ViewModels
        // Transient: chaque vue obtient son propre ViewModel
        services.AddTransient<MainViewModel>();

        // Views
        // Transient: on pourrait avoir plusieurs instances de la même vue
        services.AddTransient<MainWindow>();

        
    }
}
