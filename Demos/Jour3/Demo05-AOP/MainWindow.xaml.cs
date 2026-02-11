using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using AopDemo.Infrastructure;
using AopDemo.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AopDemo;

public partial class MainWindow : Window
{
    public AopInterceptor? Interceptor { get; private set; }

    /// <summary>
    /// Le ViewModel est injecté par le conteneur DI.
    /// L'intercepteur AOP est déjà attaché de manière transparente.
    /// </summary>
    public MainWindow(ProductViewModel viewModel)
    {
        InitializeComponent();

        // Le ViewModel reçu a déjà l'AOP branché par le conteneur
        DataContext = viewModel;

        // Récupérer l'intercepteur pour binder le log et IsDirty
        Interceptor = AopInterceptorRegistry.GetInterceptor(viewModel);

        if (Interceptor is not null)
        {
            LogList.ItemsSource = Interceptor.Log;
            Interceptor.Log.CollectionChanged += AopLog_CollectionChanged;

            // Notifier l'UI quand IsDirty change
            Interceptor.DirtyChanged += dirty =>
                DirtyIndicator.Text = dirty ? "Dirty — modifications non sauvegardées" : "Clean";
        }
    }

    private void AopLog_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && LogList.Items.Count > 0)
        {
            LogList.ScrollIntoView(LogList.Items[^1]);
        }
    }

    private void ClearLog_Click(object sender, RoutedEventArgs e)
    {
        Interceptor?.Log.Clear();
    }

    private void StartTracking_Click(object sender, RoutedEventArgs e)
    {
        Interceptor?.StartTracking();
    }
}
