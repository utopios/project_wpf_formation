using System.Windows;
using CommandsDemo.ViewModels;

namespace CommandsDemo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.LoadDataCommand.Cancel();
        }
    }
}
