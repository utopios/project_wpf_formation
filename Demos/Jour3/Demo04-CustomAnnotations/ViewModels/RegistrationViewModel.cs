using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomAnnotationsDemo.Annotations;

namespace CustomAnnotationsDemo.ViewModels;

/// <summary>
/// ViewModel démontrant l'utilisation d'annotations personnalisées
/// Formulaire d'inscription utilisateur
/// </summary>
public partial class RegistrationViewModel : ObservableValidator
{
    #region Propriétés avec annotations personnalisées

    /// <summary>
    /// Annotation standard + annotation personnalisée [ContainsNo]
    /// </summary>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
    [MinLength(3, ErrorMessage = "Minimum 3 caractères")]
    [ContainsNo("admin", "root", "system", "test")]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string _username = string.Empty;

    /// <summary>
    /// Annotation paramétrable [StrongPassword]
    /// Les paramètres sont configurables via les propriétés de l'attribut
    /// </summary>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Le mot de passe est requis")]
    [StrongPassword(MinLength = 8, RequireUppercase = true, RequireDigit = true, RequireSpecialChar = true)]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    [NotifyPropertyChangedFor(nameof(PasswordStrength))]
    private string _password = string.Empty;

    /// <summary>
    /// Annotation cross-property [NotEqualTo]
    /// Vérifie que la confirmation est différente... non, ici c'est l'inverse
    /// On utilise [CustomValidation] pour vérifier l'égalité avec Password
    /// et [NotEqualTo] sur l'ancien mot de passe
    /// </summary>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "La confirmation est requise")]
    [CustomValidation(typeof(RegistrationViewModel), nameof(ValidatePasswordConfirmation))]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string _confirmPassword = string.Empty;

    /// <summary>
    /// [NotEqualTo] : le nouveau mot de passe ne doit pas être identique à l'ancien
    /// </summary>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotEqualTo(nameof(Password), ErrorMessage = "Le nouveau mot de passe doit être différent de l'ancien")]
    private string _oldPassword = string.Empty;

    /// <summary>
    /// Annotation simple [FrenchPostalCode] - validation regex encapsulée
    /// </summary>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Le code postal est requis")]
    [FrenchPostalCode]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string _postalCode = string.Empty;

    /// <summary>
    /// Annotation avec logique dynamique [DateRange]
    /// La plage est calculée relativement à aujourd'hui
    /// </summary>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [DateRange(MinYearsAgo = 120, MaxYearsAgo = 16, ErrorMessage = "Vous devez avoir entre 16 et 120 ans")]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private DateTime? _birthDate;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isSuccess;

    #endregion

    #region Propriétés calculées

    /// <summary>
    /// Indicateur visuel de la force du mot de passe
    /// </summary>
    public string PasswordStrength
    {
        get
        {
            if (string.IsNullOrEmpty(Password)) return string.Empty;

            var score = 0;
            if (Password.Length >= 8) score++;
            if (Password.Length >= 12) score++;
            if (Password.Any(char.IsUpper)) score++;
            if (Password.Any(char.IsDigit)) score++;
            if (Password.Any(c => !char.IsLetterOrDigit(c))) score++;

            return score switch
            {
                <= 1 => "Faible",
                2 => "Moyen",
                3 => "Bon",
                4 => "Fort",
                _ => "Excellent"
            };
        }
    }

    #endregion

    #region Méthodes de validation personnalisées (CustomValidation)

    /// <summary>
    /// Validation statique pour comparer Password et ConfirmPassword
    /// Utilisée via [CustomValidation(typeof(...), nameof(...))]
    /// </summary>
    public static ValidationResult? ValidatePasswordConfirmation(string confirmPassword, ValidationContext context)
    {
        if (context.ObjectInstance is RegistrationViewModel vm)
        {
            if (!string.IsNullOrEmpty(confirmPassword) && confirmPassword != vm.Password)
            {
                return new ValidationResult("Les mots de passe ne correspondent pas");
            }
        }

        return ValidationResult.Success;
    }

    #endregion

    #region Commandes

    [RelayCommand(CanExecute = nameof(CanSubmit))]
    private void Submit()
    {
        ValidateAllProperties();

        if (HasErrors)
        {
            StatusMessage = "Veuillez corriger les erreurs avant de soumettre.";
            IsSuccess = false;
            return;
        }

        StatusMessage = $"Compte '{Username}' créé avec succès !";
        IsSuccess = true;
    }

    private bool CanSubmit()
    {
        return !HasErrors
               && !string.IsNullOrWhiteSpace(Username)
               && !string.IsNullOrWhiteSpace(Password)
               && !string.IsNullOrWhiteSpace(ConfirmPassword)
               && !string.IsNullOrWhiteSpace(PostalCode)
               && !string.IsNullOrWhiteSpace(Email);
    }

    [RelayCommand]
    private void ValidateAll()
    {
        ValidateAllProperties();

        if (HasErrors)
        {
            var errorCount = GetErrors().Count();
            StatusMessage = $"{errorCount} erreur(s) de validation trouvée(s)";
            IsSuccess = false;
        }
        else
        {
            StatusMessage = "Aucune erreur de validation !";
            IsSuccess = true;
        }
    }

    [RelayCommand]
    private void Clear()
    {
        Username = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
        OldPassword = string.Empty;
        PostalCode = string.Empty;
        BirthDate = null;
        Email = string.Empty;
        StatusMessage = string.Empty;
        ClearErrors();
    }

    [RelayCommand]
    private void FillSample()
    {
        Username = "jean.dupont";
        Password = "MonPass@2024!";
        ConfirmPassword = "MonPass@2024!";
        OldPassword = "AncienPass123";
        PostalCode = "75001";
        BirthDate = new DateTime(1990, 5, 15);
        Email = "jean.dupont@email.com";
        StatusMessage = "Données d'exemple chargées";
        IsSuccess = true;
    }

    #endregion
}
