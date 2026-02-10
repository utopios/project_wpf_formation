using System.Windows.Input;

namespace CommandsDemo.Commands;

/// <summary>
/// Implémentation réutilisable de ICommand
/// Permet de lier des actions du ViewModel aux contrôles de la View
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    /// <summary>
    /// Crée une commande qui peut toujours s'exécuter
    /// </summary>
    /// <param name="execute">Action à exécuter</param>
    public RelayCommand(Action<object?> execute)
        : this(execute, null)
    {
    }

    /// <summary>
    /// Crée une commande avec condition d'exécution
    /// </summary>
    /// <param name="execute">Action à exécuter</param>
    /// <param name="canExecute">Condition pour activer/désactiver</param>
    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Événement déclenché quand CanExecute change
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// Détermine si la commande peut s'exécuter
    /// </summary>
    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke(parameter) ?? true;
    }

    /// <summary>
    /// Exécute la commande
    /// </summary>
    public void Execute(object? parameter)
    {
        _execute(parameter);
    }

    /// <summary>
    /// Force une réévaluation de CanExecute
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}

/// <summary>
/// Version générique avec paramètre typé
/// </summary>
/// <typeparam name="T">Type du paramètre</typeparam>
public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    public RelayCommand(Action<T?> execute)
        : this(execute, null)
    {
    }

    public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
    {
        if (parameter is T typedParam)
        {
            return _canExecute?.Invoke(typedParam) ?? true;
        }
        return _canExecute?.Invoke(default) ?? true;
    }

    public void Execute(object? parameter)
    {
        if (parameter is T typedParam)
        {
            _execute(typedParam);
        }
        else
        {
            _execute(default);
        }
    }
}
