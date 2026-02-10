namespace GestionStock.ViewModels;

/// <summary>
/// ViewModel principal de l'application
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _title = "Gestion de Stock";

    [ObservableProperty]
    private bool _isNavMenuExpanded = true;

    public INavigationService NavigationService => _navigationService;

    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        // Navigation initiale vers le dashboard
        _navigationService.NavigateTo<DashboardViewModel>();

        // Écouter les messages de notification
        WeakReferenceMessenger.Default.Register<NotificationMessage>(this, OnNotificationReceived);
    }

    [ObservableProperty]
    private string? _notificationMessage;

    [ObservableProperty]
    private NotificationType _notificationType;

    [ObservableProperty]
    private bool _isNotificationVisible;

    private void OnNotificationReceived(object recipient, NotificationMessage message)
    {
        NotificationMessage = message.Message;
        NotificationType = message.Type;
        IsNotificationVisible = true;

        // Masquer automatiquement après 3 secondes
        Task.Delay(3000).ContinueWith(_ =>
        {
            Application.Current.Dispatcher.Invoke(() => IsNotificationVisible = false);
        });
    }

    [RelayCommand]
    private void NavigateTo(string viewName)
    {
        switch (viewName)
        {
            case "Dashboard":
                _navigationService.NavigateTo<DashboardViewModel>();
                break;
            case "Products":
                _navigationService.NavigateTo<ProductListViewModel>();
                break;
            case "Categories":
                _navigationService.NavigateTo<CategoryListViewModel>();
                break;
            case "Suppliers":
                _navigationService.NavigateTo<SupplierListViewModel>();
                break;
            case "Movements":
                _navigationService.NavigateTo<StockMovementViewModel>();
                break;
            case "Settings":
                _navigationService.NavigateTo<SettingsViewModel>();
                break;
        }
    }

    [RelayCommand]
    private void GoBack()
    {
        if (_navigationService.CanGoBack)
            _navigationService.GoBack();
    }

    [RelayCommand]
    private void ToggleNavMenu()
    {
        IsNavMenuExpanded = !IsNavMenuExpanded;
    }
}
