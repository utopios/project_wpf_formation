namespace GestionStock.Services;

/// <summary>
/// Service de navigation entre les vues
/// </summary>
public class NavigationService : ObservableObject, INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<NavigationEntry> _history = new();

    public event EventHandler<Type>? Navigated;

    private object? _currentViewModel;
    public object? CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    public bool CanGoBack => _history.Count > 1;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateTo<TViewModel>() where TViewModel : class
    {
        NavigateToInternal(typeof(TViewModel), null);
    }

    public void NavigateTo<TViewModel, TParameter>(TParameter parameter) where TViewModel : class
    {
        NavigateToInternal(typeof(TViewModel), parameter);
    }

    private void NavigateToInternal(Type viewModelType, object? parameter)
    {
        // Notifier l'ancien ViewModel
        if (CurrentViewModel is INavigationAware currentAware)
        {
            currentAware.OnNavigatedFrom();
        }

        // Créer le nouveau ViewModel
        var viewModel = _serviceProvider.GetRequiredService(viewModelType);

        // Ajouter à l'historique
        _history.Push(new NavigationEntry(viewModelType, parameter));

        // Notifier le nouveau ViewModel
        if (viewModel is INavigationAware newAware)
        {
            newAware.OnNavigatedTo(parameter);
        }

        CurrentViewModel = viewModel;
        OnPropertyChanged(nameof(CanGoBack));
        Navigated?.Invoke(this, viewModelType);
    }

    public void GoBack()
    {
        if (!CanGoBack) return;

        // Notifier l'ancien ViewModel
        if (CurrentViewModel is INavigationAware currentAware)
        {
            currentAware.OnNavigatedFrom();
        }

        // Retirer l'entrée actuelle
        _history.Pop();

        // Récupérer l'entrée précédente
        var previous = _history.Peek();

        // Recréer le ViewModel précédent
        var viewModel = _serviceProvider.GetRequiredService(previous.ViewModelType);

        if (viewModel is INavigationAware newAware)
        {
            newAware.OnNavigatedTo(previous.Parameter);
        }

        CurrentViewModel = viewModel;
        OnPropertyChanged(nameof(CanGoBack));
        Navigated?.Invoke(this, previous.ViewModelType);
    }

    private record NavigationEntry(Type ViewModelType, object? Parameter);
}
