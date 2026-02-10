namespace GestionStock.Views;

/// <summary>
/// Fenêtre principale de l'application
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
