using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ToolkitDemo.ViewModels;

/// <summary>
/// ViewModel utilisant CommunityToolkit.Mvvm
/// Démontre les générateurs de source: [ObservableProperty] et [RelayCommand]
///
/// IMPORTANT: La classe doit être `partial` pour que les générateurs fonctionnent
/// </summary>
public partial class PersonViewModel : ObservableObject
{
    /// <summary>
    /// [ObservableProperty] génère automatiquement:
    /// - Une propriété publique FirstName (PascalCase)
    /// - Le getter et setter avec notification INPC
    /// - Le champ backing _firstName
    ///
    /// [NotifyPropertyChangedFor] notifie automatiquement les propriétés dépendantes
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullName))]
    [NotifyPropertyChangedFor(nameof(Greeting))]
    private string _firstName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullName))]
    [NotifyPropertyChangedFor(nameof(Greeting))]
    private string _lastName = string.Empty;

    /// <summary>
    /// [NotifyCanExecuteChangedFor] rafraîchit automatiquement CanExecute
    /// de la commande quand la propriété change
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAdult))]
    [NotifyPropertyChangedFor(nameof(AgeCategory))]
    [NotifyCanExecuteChangedFor(nameof(IncrementAgeCommand))]
    [NotifyCanExecuteChangedFor(nameof(DecrementAgeCommand))]
    private int _age;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _message = string.Empty;

    #region Propriétés calculées

    /// <summary>
    /// Propriétés en lecture seule calculées à partir d'autres propriétés
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    public string Greeting => string.IsNullOrWhiteSpace(FullName)
        ? "Entrez votre nom"
        : $"Bonjour, {FullName} !";

    public bool IsAdult => Age >= 18;

    public string AgeCategory => Age switch
    {
        < 13 => "Enfant",
        < 18 => "Adolescent",
        < 65 => "Adulte",
        _ => "Senior"
    };

    #endregion

    #region Commandes générées avec [RelayCommand]

    /// <summary>
    /// [RelayCommand] génère automatiquement:
    /// - Une propriété IRelayCommand SayHelloCommand
    /// - L'instanciation dans un constructeur partiel
    ///
    /// Le nom de la commande = nom de la méthode + "Command"
    /// </summary>
    [RelayCommand]
    private void SayHello()
    {
        Message = $"Bonjour {FullName} ! Vous avez {Age} ans.";
    }

    /// <summary>
    /// Commande avec CanExecute
    /// La méthode CanIncrementAge() est automatiquement appelée
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanIncrementAge))]
    private void IncrementAge()
    {
        Age++;
    }

    private bool CanIncrementAge() => Age < 120;

    [RelayCommand(CanExecute = nameof(CanDecrementAge))]
    private void DecrementAge()
    {
        Age--;
    }

    private bool CanDecrementAge() => Age > 0;

    /// <summary>
    /// Commande async - génère automatiquement un IAsyncRelayCommand
    /// avec gestion de IsExecuting et annulation
    /// </summary>
    [RelayCommand]
    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        Message = "Chargement en cours...";

        for (int i = 1; i <= 5; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Run(() => { 
            //Logique
            });
            await Task.Delay(500, cancellationToken);
            Message = $"Étape {i}/5 terminée...";
        }

        Message = "Chargement terminé !";
    }

    /// <summary>
    /// Commande avec paramètre
    /// Le type du paramètre est déduit de la signature
    /// </summary>
    [RelayCommand]
    private void SetAge(int newAge)
    {
        Age = newAge;
        Message = $"Âge défini à {newAge} ans";
    }

    [RelayCommand]
    private void Reset()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Age = 0;
        Email = string.Empty;
        Message = "Formulaire réinitialisé";
    }

    #endregion
}
