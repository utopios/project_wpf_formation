namespace DIDemo.Services;

/// <summary>
/// Interface pour la navigation entre vues
/// </summary>
public interface INavigationService
{
    void NavigateTo<TViewModel>() where TViewModel : class;
    event EventHandler<Type>? Navigated;
}

/// <summary>
/// Implémentation simple du service de navigation
/// </summary>
public class NavigationService : INavigationService
{
    public event EventHandler<Type>? Navigated;

    public void NavigateTo<TViewModel>() where TViewModel : class
    {
        Navigated?.Invoke(this, typeof(TViewModel));
    }
}
