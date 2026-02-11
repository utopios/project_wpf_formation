using System.Windows;
using ConfigDemo.ViewModels;

namespace ConfigDemo;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
