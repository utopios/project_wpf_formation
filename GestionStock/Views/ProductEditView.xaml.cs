using GestionStock.Models;
using GestionStock.ViewModels;

namespace GestionStock.Views;

/// <summary>
/// Fenêtre d'édition de produit (MVVM sans toolkit).
/// Le code-behind ne fait que créer le ViewModel et écouter la fermeture.
/// </summary>
public partial class ProductEditView : Window
{
    public ProductEditView(Product? product = null)
    {
        InitializeComponent();
        var viewModel = new ProductEditFormViewModel(product);
        //viewModel.CloseRequested += result => DialogResult = result;
        viewModel.CloseRequested += Close;
        //viewModel.CloseRequested += Close;
        DataContext = viewModel;
    }

    private void Close(bool result)
    {
        DialogResult = result;
    }
    
    ~ProductEditView()
    {
        ((ProductEditFormViewModel)DataContext).CloseRequested -= Close;
    }

    
}
