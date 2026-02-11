using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DIDemo.Services;

namespace DIDemo.ViewModels;

/// <summary>
/// ViewModel principal avec injection de dépendances
/// Les services sont injectés via le constructeur
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IMessageService _messageService;
    private readonly IDataService _dataService;

    /// <summary>
    /// Injection par constructeur - les dépendances sont explicites
    /// Le conteneur DI les résout automatiquement
    /// </summary>
    public MainViewModel(IMessageService messageService, IDataService dataService)
    {
        _messageService = messageService;
        _dataService = dataService;

        // Charger les données au démarrage
        _ = LoadItemsAsync();
    }

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _currentMessage = string.Empty;

    [ObservableProperty]
    private string _newItem = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public ObservableCollection<string> Items { get; } = [];
    public ObservableCollection<string> Logs { get; } = [];

    [RelayCommand]
    private void SayHello()
    {
        if (string.IsNullOrWhiteSpace(UserName))
        {
            CurrentMessage = "Veuillez entrer votre nom.";
            return;
        }

        CurrentMessage = _messageService.GetGreeting(UserName);
        RefreshLogs();
    }

    [RelayCommand]
    private void SayGoodbye()
    {
        if (string.IsNullOrWhiteSpace(UserName))
        {
            CurrentMessage = "Veuillez entrer votre nom.";
            return;
        }

        CurrentMessage = _messageService.GetFarewell(UserName);
        RefreshLogs();
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        IsLoading = true;

        try
        {
            Items.Clear();
            var items = await _dataService.GetItemsAsync();

            foreach (var item in items)
            {
                Items.Add(item);
            }

            Logs.Insert(0, $"[{DateTime.Now:HH:mm:ss}] Chargé {Items.Count} items");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddItemAsync()
    {
        if (string.IsNullOrWhiteSpace(NewItem))
            return;

        await _dataService.AddItemAsync(NewItem);
        Items.Add(NewItem);
        Logs.Insert(0, $"[{DateTime.Now:HH:mm:ss}] Ajouté: {NewItem}");
        NewItem = string.Empty;
    }

    private void RefreshLogs()
    {
        foreach (var message in _messageService.GetMessages().Reverse().Take(5))
        {
            if (!Logs.Contains(message))
            {
                Logs.Insert(0, message);
            }
        }
    }
}
