namespace DIDemo.Services;

/// <summary>
/// Interface pour le service de données
/// </summary>
public interface IDataService
{
    Task<IEnumerable<string>> GetItemsAsync();
    Task AddItemAsync(string item);
    int ItemCount { get; }
}

/// <summary>
/// Implémentation simulée du service de données
/// Dans une vraie application, utiliserait Entity Framework ou une API
/// </summary>
public class DataService : IDataService
{
    private readonly List<string> _items = ["Item initial 1", "Item initial 2", "Item initial 3"];

    public int ItemCount => _items.Count;

    public Task<IEnumerable<string>> GetItemsAsync()
    {
        // Simuler un délai réseau
        return Task.FromResult<IEnumerable<string>>(_items.ToList());
    }

    public Task AddItemAsync(string item)
    {
        _items.Add(item);
        return Task.CompletedTask;
    }
}
