using System.Windows;
using AopDemo.Infrastructure;
using AopDemo.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AopDemo;

public partial class App : Application
{
    private static IServiceProvider _services = null!;
    public static IServiceProvider Services => _services;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        // ── Configuration DI (comme Spring @Configuration) ──
        var services = new ServiceCollection();

        // Enregistrer le ViewModel AVEC AOP transparent
        // C'est ici que l'interception est branchée — le ViewModel ne le sait pas
        services.AddWithAop<ProductViewModel>();

        // Enregistrer la MainWindow
        services.AddSingleton<MainWindow>();

        _services = services.BuildServiceProvider();

        // Lancer la fenêtre via DI
        var mainWindow = _services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
