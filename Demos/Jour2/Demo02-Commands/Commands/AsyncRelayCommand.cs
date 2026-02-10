using System.Windows.Input;

namespace CommandsDemo.Commands;

/// <summary>
/// Commande asynchrone pour les opérations longues
/// Gère IsExecuting et empêche les exécutions concurrentes
/// </summary>
public class AsyncRelayCommand : ICommand
{
    private readonly Func<CancellationToken, Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool _isExecuting;
    private CancellationTokenSource? _cancellationTokenSource;

    /// <summary>
    /// Indique si la commande est en cours d'exécution
    /// </summary>
    public bool IsExecuting
    {
        get => _isExecuting;
        private set
        {
            _isExecuting = value;
            RaiseCanExecuteChanged();
            IsExecutingChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Événement déclenché quand IsExecuting change
    /// </summary>
    public event EventHandler? IsExecutingChanged;

    public AsyncRelayCommand(Func<Task> execute)
        : this(_ => execute(), null)
    {
    }

    public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute)
        : this(_ => execute(), canExecute)
    {
    }

    public AsyncRelayCommand(Func<CancellationToken, Task> execute)
        : this(execute, null)
    {
    }

    public AsyncRelayCommand(Func<CancellationToken, Task> execute, Func<bool>? canExecute)
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
        // Ne peut pas s'exécuter si déjà en cours
        if (IsExecuting)
            return false;

        return _canExecute?.Invoke() ?? true;
    }

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            return;

        IsExecuting = true;
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await _execute(_cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            // Annulation normale, ne pas propager
        }
        finally
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            IsExecuting = false;
        }
    }

    /// <summary>
    /// Annule l'exécution en cours
    /// </summary>
    public void Cancel()
    {
        _cancellationTokenSource?.Cancel();
    }

    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}

/// <summary>
/// Version générique avec paramètre typé
/// </summary>
public class AsyncRelayCommand<T> : ICommand
{
    private readonly Func<T?, CancellationToken, Task> _execute;
    private readonly Func<T?, bool>? _canExecute;
    private bool _isExecuting;
    private CancellationTokenSource? _cancellationTokenSource;

    public bool IsExecuting
    {
        get => _isExecuting;
        private set
        {
            _isExecuting = value;
            RaiseCanExecuteChanged();
            IsExecutingChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? IsExecutingChanged;

    public AsyncRelayCommand(Func<T?, Task> execute)
        : this((p, _) => execute(p), null)
    {
    }

    public AsyncRelayCommand(Func<T?, Task> execute, Func<T?, bool>? canExecute)
        : this((p, _) => execute(p), canExecute)
    {
    }

    public AsyncRelayCommand(Func<T?, CancellationToken, Task> execute, Func<T?, bool>? canExecute = null)
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
        if (IsExecuting)
            return false;

        if (parameter is T typedParam)
        {
            return _canExecute?.Invoke(typedParam) ?? true;
        }
        return _canExecute?.Invoke(default) ?? true;
    }

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            return;

        IsExecuting = true;
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            T? typedParam = parameter is T p ? p : default;
            await _execute(typedParam, _cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            // Annulation normale
        }
        finally
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            IsExecuting = false;
        }
    }

    public void Cancel()
    {
        _cancellationTokenSource?.Cancel();
    }

    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}
