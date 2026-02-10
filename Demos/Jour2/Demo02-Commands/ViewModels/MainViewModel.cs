using System.Collections.ObjectModel;
using System.Windows.Input;
using CommandsDemo.Commands;

namespace CommandsDemo.ViewModels;

public class MainViewModel : ViewModelBase
{
    private int _counter;
    private string _message = string.Empty;
    private string _newItem = string.Empty;
    private string? _selectedItem;
    private bool _isLoading;
    private int _progress;

    public MainViewModel()
    {
        // Commandes synchrones
        IncrementCommand = new RelayCommand(_ => Counter++);
        DecrementCommand = new RelayCommand(_ => Counter--, _ => Counter > 0);
        ResetCommand = new RelayCommand(_ => Counter = 0, _ => Counter != 0);

        // Commande avec paramètre
        SetValueCommand = new RelayCommand<int>(value => Counter = value);

        // Commandes pour la liste
        AddItemCommand = new RelayCommand(_ => AddItem(), _ => !string.IsNullOrWhiteSpace(NewItem));
        RemoveItemCommand = new RelayCommand<string>(RemoveItem, item => item != null);
        ClearListCommand = new RelayCommand(_ => Items.Clear(), _ => Items.Count > 0);

        // Commande asynchrone
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);

        // Initialiser quelques items
        Items.Add("Premier élément");
        Items.Add("Deuxième élément");
        Items.Add("Troisième élément");
    }

    #region Propriétés

    public int Counter
    {
        get => _counter;
        set
        {
            if (SetProperty(ref _counter, value))
            {
                OnPropertyChanged(nameof(CounterDisplay));
            }
        }
    }

    public string CounterDisplay => Counter switch
    {
        0 => "Zéro",
        1 => "Un",
        < 0 => $"Négatif ({Counter})",
        _ => Counter.ToString()
    };

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public string NewItem
    {
        get => _newItem;
        set => SetProperty(ref _newItem, value);
    }

    public string? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public ObservableCollection<string> Items { get; } = new();

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public int Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }

    #endregion

    #region Commandes

    // Commandes synchrones simples
    public ICommand IncrementCommand { get; }
    public ICommand DecrementCommand { get; }
    public ICommand ResetCommand { get; }

    // Commande avec paramètre
    public RelayCommand<int> SetValueCommand { get; }

    // Commandes pour la liste
    public ICommand AddItemCommand { get; }
    public RelayCommand<string> RemoveItemCommand { get; }
    public ICommand ClearListCommand { get; }

    // Commande asynchrone
    public AsyncRelayCommand LoadDataCommand { get; }

    #endregion

    #region Méthodes

    private void AddItem()
    {
        if (!string.IsNullOrWhiteSpace(NewItem))
        {
            Items.Add(NewItem);
            NewItem = string.Empty;
        }
    }

    private void RemoveItem(string? item)
    {
        if (item != null)
        {
            Items.Remove(item);
        }
    }

    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        IsLoading = true;
        Progress = 0;
        Message = "Chargement en cours...";

        try
        {
            // Simuler un chargement progressif
            for (int i = 0; i <= 100; i += 10)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Progress = i;
                await Task.Delay(300, cancellationToken);
            }

            Message = "Chargement terminé avec succès !";
        }
        catch (OperationCanceledException)
        {
            Message = "Chargement annulé.";
            Progress = 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion
}
