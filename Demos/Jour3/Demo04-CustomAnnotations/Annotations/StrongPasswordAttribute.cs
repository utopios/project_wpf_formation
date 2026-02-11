using System.ComponentModel.DataAnnotations;

namespace CustomAnnotationsDemo.Annotations;

/// <summary>
/// Annotation paramétrable
/// Valide la robustesse d'un mot de passe avec des critères configurables
///
/// Utilisation: [StrongPassword]
///          ou: [StrongPassword(MinLength = 10, RequireSpecialChar = false)]
/// </summary>
public class StrongPasswordAttribute : ValidationAttribute
{
    /// <summary>Longueur minimale (défaut: 8)</summary>
    public int MinLength { get; set; } = 8;

    /// <summary>Exiger au moins une majuscule (défaut: true)</summary>
    public bool RequireUppercase { get; set; } = true;

    /// <summary>Exiger au moins un chiffre (défaut: true)</summary>
    public bool RequireDigit { get; set; } = true;

    /// <summary>Exiger au moins un caractère spécial (défaut: true)</summary>
    public bool RequireSpecialChar { get; set; } = true;

    /// <summary>
    /// Surcharge avec ValidationContext pour un message d'erreur détaillé
    /// </summary>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string password || string.IsNullOrWhiteSpace(password))
            return ValidationResult.Success; // Null géré par [Required]

        var errors = new List<string>();

        if (password.Length < MinLength)
            errors.Add($"au moins {MinLength} caractères");

        if (RequireUppercase && !password.Any(char.IsUpper))
            errors.Add("une majuscule");

        if (RequireDigit && !password.Any(char.IsDigit))
            errors.Add("un chiffre");

        if (RequireSpecialChar && password.All(char.IsLetterOrDigit))
            errors.Add("un caractère spécial (!@#$...)");

        if (errors.Count > 0)
        {
            var message = $"Le mot de passe doit contenir : {string.Join(", ", errors)}";
            return new ValidationResult(message);
        }

        return ValidationResult.Success;
    }
}
