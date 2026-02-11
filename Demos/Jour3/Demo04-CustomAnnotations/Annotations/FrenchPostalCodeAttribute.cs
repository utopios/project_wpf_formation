using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CustomAnnotationsDemo.Annotations;

/// <summary>
/// Annotation simple sans paramètre
/// Valide qu'une chaîne est un code postal français (5 chiffres)
///
/// Utilisation: [FrenchPostalCode]
/// </summary>
public class FrenchPostalCodeAttribute : ValidationAttribute
{
    public FrenchPostalCodeAttribute()
        : base("Le code postal doit contenir 5 chiffres (ex: 75001)")
    {
    }

    /// <summary>
    /// Méthode principale de validation
    /// Retourne true si la valeur est valide, false sinon
    /// </summary>
    public override bool IsValid(object? value)
    {
        // Null est considéré valide (utiliser [Required] pour l'obligation)
        if (value is not string postalCode || string.IsNullOrWhiteSpace(postalCode))
            return true;

        return Regex.IsMatch(postalCode, @"^\d{5}$");
    }
}
