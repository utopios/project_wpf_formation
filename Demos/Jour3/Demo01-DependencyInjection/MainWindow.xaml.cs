using System.Windows;
using DIDemo.ViewModels;

namespace DIDemo;

public partial class MainWindow : Window
{
    /// <summary>
    /// Le ViewModel est injecté par le constructeur
    /// Le conteneur DI résout automatiquement MainViewModel
    /// et ses dépendances (IMessageService, IDataService)
    /// </summary>
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
