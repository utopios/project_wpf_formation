using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace AopDemo.Infrastructure;

/// <summary>
/// Extensions DI pour l'AOP transparent.
///
/// Comme Spring AOP : le conteneur crée l'objet, attache l'intercepteur,
/// et retourne l'objet "enrichi". Le ViewModel ne sait pas qu'il est intercepté.
///
/// Utilisation :
///   services.AddWithAop&lt;ProductViewModel&gt;();
///
/// Equivalent Spring :
///   @Component + @Aspect → le proxy est créé automatiquement par le conteneur
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Enregistre un ViewModel avec interception AOP automatique.
    /// A la résolution, l'intercepteur est attaché de manière transparente.
    /// </summary>
    public static IServiceCollection AddWithAop<T>(this IServiceCollection services)
        where T : class, INotifyPropertyChanged
    {
        // Enregistrer le ViewModel via une factory
        // La factory crée l'instance ET attache l'intercepteur
        services.AddTransient<T>(sp =>
        {
            // 1. Créer le ViewModel normalement
            var viewModel = ActivatorUtilities.CreateInstance<T>(sp);

            // 2. Attacher l'intercepteur AOP (transparent pour le ViewModel)
            var interceptor = AopInterceptor.Attach(viewModel);

            // 3. Stocker l'intercepteur pour que l'UI puisse y accéder
            //    (pour le log et IsDirty)
            AopInterceptorRegistry.Register(viewModel, interceptor);

            return viewModel;
        });

        return services;
    }
}

/// <summary>
/// Registre global des intercepteurs.
/// Permet à l'UI de récupérer l'intercepteur associé à un ViewModel
/// pour binder le Log et IsDirty.
///
/// C'est l'équivalent de demander au conteneur Spring le proxy d'un bean.
/// </summary>
public static class AopInterceptorRegistry
{
    // WeakReference pour ne pas empêcher le GC
    private static readonly Dictionary<int, AopInterceptor> _interceptors = new();

    public static void Register(object target, AopInterceptor interceptor)
    {
        _interceptors[target.GetHashCode()] = interceptor;
    }

    /// <summary>
    /// Récupère l'intercepteur attaché à un ViewModel.
    /// Utilisé dans le code-behind pour binder le log.
    /// </summary>
    public static AopInterceptor? GetInterceptor(object target)
    {
        _interceptors.TryGetValue(target.GetHashCode(), out var interceptor);
        return interceptor;
    }
}
