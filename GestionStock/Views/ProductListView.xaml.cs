using System.Windows;
using System.Windows.Controls;
using GestionStock.Models;
using Microsoft.Extensions.DependencyInjection;

namespace GestionStock.Views;

public partial class ProductListView : UserControl
{
    public ProductListView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gestionnaire d'événement pour le bouton "Voir" - Ouvre la fenêtre de détails du produit
    /// </summary>
    private void ViewProduct_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Product product)
        {
            var detailWindow = new ProductDetailView(product);
            detailWindow.ShowDialog();
        }
    }

    /// <summary>
    /// Gestionnaire d'événement pour le bouton "Modifier" - Ouvre la fenêtre d'édition du produit
    /// </summary>
    private void EditProduct_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Product product)
        {
            var editWindow = new ProductEditView(product);
            if (editWindow.ShowDialog() == true)
            {
                // Recharger la liste via le ViewModel
                if (DataContext is ViewModels.ProductListViewModel viewModel)
                {
                    viewModel.LoadDataCommand.Execute(null);
                }
            }
        }
    }

    /// <summary>
    /// Gestionnaire d'événement pour le bouton "Supprimer" - Supprime le produit après confirmation
    /// </summary>
    private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Product product)
        {
            var result = MessageBox.Show(
                $"Voulez-vous vraiment supprimer le produit '{product.Name}' ?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Utiliser App.Services pour obtenir le repository
                    var productRepository = App.Services.GetRequiredService<Services.IProductRepository>();
                    await productRepository.DeleteAsync(product.Id);

                    MessageBox.Show(
                        "Produit supprimé avec succès",
                        "Succès",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Recharger la liste via le ViewModel
                    if (DataContext is ViewModels.ProductListViewModel viewModel)
                    {
                        viewModel.LoadDataCommand.Execute(null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Erreur lors de la suppression : {ex.Message}",
                        "Erreur",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
    }
}
