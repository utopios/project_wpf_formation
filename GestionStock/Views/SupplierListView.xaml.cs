using System.Windows;
using System.Windows.Controls;
using GestionStock.Models;

namespace GestionStock.Views;

public partial class SupplierListView : UserControl
{
    public SupplierListView()
    {
        InitializeComponent();
    }

    private void EditSupplier_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Supplier supplier)
        {
            if (DataContext is ViewModels.SupplierListViewModel viewModel)
            {
                viewModel.SelectedSupplier = supplier;
                viewModel.EditSupplierCommand.Execute(null);
            }
        }
    }

    private void DeleteSupplier_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Supplier supplier)
        {
            if (DataContext is ViewModels.SupplierListViewModel viewModel)
            {
                viewModel.SelectedSupplier = supplier;
                viewModel.DeleteSupplierCommand.Execute(null);
            }
        }
    }
}
