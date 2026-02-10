using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVMBaseDemo.ViewModels;

/// <summary>
/// Classe de base pour tous les ViewModels
/// Implémente INotifyPropertyChanged avec le pattern SetProperty
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Déclenche l'événement PropertyChanged
    /// </summary>
    /// <param name="propertyName">Nom de la propriété (automatique avec CallerMemberName)</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Met à jour une propriété et notifie si la valeur change
    /// Pattern standard pour éviter les répétitions
    /// </summary>
    /// <typeparam name="T">Type de la propriété</typeparam>
    /// <param name="field">Référence au champ backing</param>
    /// <param name="value">Nouvelle valeur</param>
    /// <param name="propertyName">Nom de la propriété (automatique)</param>
    /// <returns>True si la valeur a changé</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        // Si la valeur n'a pas changé, ne rien faire
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        // Mettre à jour et notifier
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Version avec callback appelé après le changement
    /// Utile pour les propriétés calculées ou les validations
    /// </summary>
    protected bool SetProperty<T>(ref T field, T value, Action onChanged,
        [CallerMemberName] string? propertyName = null)
    {
        if (SetProperty(ref field, value, propertyName))
        {
            onChanged?.Invoke();
            return true;
        }
        return false;
    }
}
