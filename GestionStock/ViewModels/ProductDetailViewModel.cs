namespace GestionStock.ViewModels;

/// <summary>
/// ViewModel pour le détail d'un produit
/// </summary>
public partial class ProductDetailViewModel : NavigableViewModelBase
{
    private readonly IProductRepository _productRepository;
    private readonly IStockService _stockService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    private int _productId;

    [ObservableProperty]
    private Product? _product;

    [ObservableProperty]
    private ObservableCollection<StockMovement> _movements = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddStockCommand))]
    [NotifyCanExecuteChangedFor(nameof(RemoveStockCommand))]
    private int _stockQuantity;

    [ObservableProperty]
    private string? _stockReason;

    public ProductDetailViewModel(
        IProductRepository productRepository,
        IStockService stockService,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _productRepository = productRepository;
        _stockService = stockService;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    public override void OnNavigatedTo(object? parameter)
    {
        if (parameter is int productId)
        {
            _productId = productId;
            LoadProductCommand.Execute(null);
        }
    }

    [RelayCommand]
    private async Task LoadProductAsync()
    {
        await ExecuteAsync(async () =>
        {
            Product = await _productRepository.GetByIdAsync(_productId);

            if (Product != null)
            {
                var movements = await _stockService.GetMovementsForProductAsync(_productId);
                Movements = new ObservableCollection<StockMovement>(movements);
            }
        });
    }

    [RelayCommand]
    private void Edit()
    {
        _navigationService.NavigateTo<ProductEditViewModel, int>(_productId);
    }

    private bool CanModifyStock => StockQuantity > 0 && Product != null;

    [RelayCommand(CanExecute = nameof(CanModifyStock))]
    private async Task AddStockAsync()
    {
        if (Product == null) return;

        await ExecuteAsync(async () =>
        {
            await _stockService.AddStockAsync(_productId, StockQuantity, StockReason);
            await LoadProductAsync();
            StockQuantity = 0;
            StockReason = null;
            _dialogService.ShowNotification($"Stock ajouté: +{StockQuantity} unités", NotificationType.Success);
        });
    }

    [RelayCommand(CanExecute = nameof(CanModifyStock))]
    private async Task RemoveStockAsync()
    {
        if (Product == null) return;

        if (StockQuantity > Product.QuantityInStock)
        {
            await _dialogService.AlertAsync("Erreur", "Stock insuffisant pour cette sortie.");
            return;
        }

        await ExecuteAsync(async () =>
        {
            await _stockService.RemoveStockAsync(_productId, StockQuantity, StockReason);
            await LoadProductAsync();
            StockQuantity = 0;
            StockReason = null;
            _dialogService.ShowNotification($"Stock retiré: -{StockQuantity} unités", NotificationType.Success);
        });
    }

    [RelayCommand]
    private async Task AdjustStockAsync()
    {
        if (Product == null) return;

        var input = await _dialogService.PromptAsync(
            "Ajustement de stock",
            "Entrez la nouvelle quantité en stock:",
            Product.QuantityInStock.ToString());

        if (input != null && int.TryParse(input, out var newQuantity) && newQuantity >= 0)
        {
            await ExecuteAsync(async () =>
            {
                await _stockService.AdjustStockAsync(_productId, newQuantity, "Ajustement manuel");
                await LoadProductAsync();
                _dialogService.ShowNotification("Stock ajusté avec succès", NotificationType.Success);
            });
        }
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.GoBack();
    }
}
