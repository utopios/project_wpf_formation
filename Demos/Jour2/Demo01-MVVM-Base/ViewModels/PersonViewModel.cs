namespace MVVMBaseDemo.ViewModels;

/// <summary>
/// ViewModel pour une personne
/// Démontre INotifyPropertyChanged et les propriétés calculées
/// </summary>
public class PersonViewModel : ViewModelBase
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private int _age;
    private string _email = string.Empty;

    /// <summary>
    /// Prénom - Notifie NomComplet quand changé
    /// </summary>
    public string FirstName
    {
        get => _firstName;
        set
        {
            if (SetProperty(ref _firstName, value))
            {
                // Notifier que FullName a aussi changé
                OnPropertyChanged(nameof(FullName));
                OnPropertyChanged(nameof(Greeting));
            }
        }
    }

    /// <summary>
    /// Nom - Notifie NomComplet quand changé
    /// </summary>
    public string LastName
    {
        get => _lastName;
        set
        {
            if (SetProperty(ref _lastName, value))
            {
                // Notifier que FullName a aussi changé
                OnPropertyChanged(nameof(FullName));
                OnPropertyChanged(nameof(Greeting));
            }
        }
    }

    /// <summary>
    /// Âge
    /// </summary>
    public int Age
    {
        get => _age;
        set
        {
            if (SetProperty(ref _age, value))
            {
                OnPropertyChanged(nameof(IsAdult));
                OnPropertyChanged(nameof(AgeCategory));
            }
        }
    }

    /// <summary>
    /// Email
    /// </summary>
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    #region Propriétés calculées (lecture seule)

    /// <summary>
    /// Nom complet - Propriété calculée
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Message de bienvenue
    /// </summary>
    public string Greeting => string.IsNullOrWhiteSpace(FullName)
        ? "Bienvenue !"
        : $"Bonjour, {FullName} !";

    /// <summary>
    /// Indique si la personne est adulte
    /// </summary>
    public bool IsAdult => Age >= 18;

    /// <summary>
    /// Catégorie d'âge
    /// </summary>
    public string AgeCategory => Age switch
    {
        < 0 => "Invalide",
        < 13 => "Enfant",
        < 18 => "Adolescent",
        < 65 => "Adulte",
        _ => "Senior"
    };

    #endregion
}
