using System.ComponentModel.DataAnnotations;

namespace CustomAnnotationsDemo.Annotations;

/// <summary>
/// Annotation cross-property (validation entre propriétés)
/// Vérifie que la valeur est différente d'une autre propriété
///
/// Utilisation: [NotEqualTo(nameof(OtherProperty), ErrorMessage = "...")]
///
/// Cas typique: le nouveau mot de passe ne doit pas être identique à l'ancien
/// </summary>
public class NotEqualToAttribute : ValidationAttribute
{
    private readonly string _otherPropertyName;

    public NotEqualToAttribute(string otherPropertyName)
        : base("La valeur ne peut pas être identique à {0}")
    {
        _otherPropertyName = otherPropertyName;
    }

    /// <summary>
    /// Utilise le ValidationContext pour accéder à l'instance de l'objet
    /// et comparer avec l'autre propriété
    /// </summary>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        // Récupérer la propriété cible via réflexion
        var otherProperty = validationContext.ObjectType.GetProperty(_otherPropertyName);

        if (otherProperty is null)
            throw new ArgumentException($"Propriété '{_otherPropertyName}' introuvable");

        var otherValue = otherProperty.GetValue(validationContext.ObjectInstance);

        if (value.Equals(otherValue))
        {
            var displayName = otherProperty.Name;
            var message = FormatErrorMessage(displayName);
            return new ValidationResult(message);
        }

        return ValidationResult.Success;
    }
}
