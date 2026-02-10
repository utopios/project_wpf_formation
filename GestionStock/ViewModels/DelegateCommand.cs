using System.Windows.Input;

namespace GestionStock.ViewModels;

/// <summary>
/// Implémentation manuelle de ICommand pour le MVVM sans toolkit.
/// Permet de binder des actions du ViewModel aux boutons du XAML.
/// </summary>
public class DelegateCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    /// <summary>
    /// Crée une commande avec une action et une condition optionnelle.
    /// </summary>
    /// <param name="execute">Action à exécuter</param>
    /// <param name="canExecute">Condition pour activer/désactiver le bouton (optionnel)</param>
    public DelegateCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        //_execute = execute !=  null ? execute : throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Événement déclenché quand la condition CanExecute peut avoir changé.
    /// Lié au CommandManager de WPF qui réévalue automatiquement lors des interactions UI.
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// Détermine si la commande peut être exécutée.
    /// </summary>
    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute(parameter);
    }

    /// <summary>
    /// Exécute la commande.
    /// </summary>
    public void Execute(object? parameter)
    {
        _execute(parameter);
    }

    /// <summary>
    /// Force WPF à réévaluer CanExecute sur toutes les commandes.
    /// </summary>
    public static void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}
