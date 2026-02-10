using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GestionStock.ViewModels;

/// <summary>
/// Classe de base pour le MVVM sans toolkit.
/// Implémente INotifyPropertyChanged manuellement.
/// </summary>
public abstract class BindableBase : INotifyPropertyChanged
{
    /// <summary>
    /// Événement déclenché quand une propriété change.
    /// L'UI (WPF) s'abonne automatiquement à cet événement via les Bindings.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Déclenche l'événement PropertyChanged pour notifier l'UI.
    /// </summary>
    /// <param name="propertyName">Nom de la propriété (rempli automatiquement par CallerMemberName)</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Met à jour un champ et notifie l'UI si la valeur a changé.
    /// </summary>
    /// <typeparam name="T">Type de la propriété</typeparam>
    /// <param name="field">Référence vers le champ privé</param>
    /// <param name="value">Nouvelle valeur</param>
    /// <param name="propertyName">Nom de la propriété (automatique)</param>
    /// <returns>true si la valeur a changé, false sinon</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
