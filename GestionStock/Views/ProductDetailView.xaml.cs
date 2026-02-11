using GestionStock.Models;
using GestionStock.ViewModels;

namespace GestionStock.Views;

/// <summary>
/// Fenêtre de détails d'un produit (MVVM avec CommunityToolkit).
/// Le code-behind ne fait que créer le ViewModel et gérer la fermeture.
/// </summary>
public partial class ProductDetailView : Window
{
    public ProductDetailView(Product product)
    {
        InitializeComponent();
        var viewModel = new ProductInfoViewModel(product);
        viewModel.CloseRequested += Close;
        DataContext = viewModel;
        Closed += OnClosed;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        ((ProductInfoViewModel)DataContext).CloseRequested -= Close;
    }

    private void Close(bool result)
    {
        DialogResult = result;
    }

    //public void Dispose()
    //{
    //    ((ProductInfoViewModel)DataContext).CloseRequested -= Close;
    //}

    


    ~ProductDetailView()
    {
        //Dispatcher.Invoke(() =>
        //{
        //    ((ProductInfoViewModel)DataContext).CloseRequested -= Close;
        //});
       
    }
}
