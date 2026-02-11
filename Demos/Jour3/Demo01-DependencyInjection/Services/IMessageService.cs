namespace DIDemo.Services;

/// <summary>
/// Interface pour le service de messages
/// Démontre la programmation contre des interfaces (DI)
/// </summary>
public interface IMessageService
{
    string GetGreeting(string name);
    string GetFarewell(string name);
    IEnumerable<string> GetMessages();
}

/// <summary>
/// Implémentation de production
/// </summary>
public class MessageService : IMessageService
{
    private readonly List<string> _messages = [];

    public string GetGreeting(string name)
    {
        var message = $"Bonjour {name} ! Bienvenue dans l'application.";
        _messages.Add($"[{DateTime.Now:HH:mm:ss}] Greeting: {message}");
        return message;
    }

    public string GetFarewell(string name)
    {
        var message = $"Au revoir {name} ! À bientôt.";
        _messages.Add($"[{DateTime.Now:HH:mm:ss}] Farewell: {message}");
        return message;
    }

    public IEnumerable<string> GetMessages() => _messages.AsReadOnly();
}
