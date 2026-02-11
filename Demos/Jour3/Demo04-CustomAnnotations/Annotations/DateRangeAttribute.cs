using System.ComponentModel.DataAnnotations;

namespace CustomAnnotationsDemo.Annotations;

/// <summary>
/// Annotation avec logique relative (dates dynamiques)
/// Vérifie qu'une date est dans une plage relative à aujourd'hui
///
/// Utilisation: [DateRange(MinYearsAgo = 120, MaxYearsAgo = 0)]  → date de naissance
///          ou: [DateRange(MinYearsAgo = -1, MaxYearsAgo = -10)]  → date future (1 à 10 ans)
///
/// Les valeurs négatives représentent le futur
/// </summary>
public class DateRangeAttribute : ValidationAttribute
{
    /// <summary>Nombre minimum d'années dans le passé (défaut: 150)</summary>
    public int MinYearsAgo { get; set; } = 150;

    /// <summary>Nombre maximum d'années dans le passé. 0 = aujourd'hui, négatif = futur</summary>
    public int MaxYearsAgo { get; set; } = 0;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateTime date)
            return ValidationResult.Success;

        var today = DateTime.Today;
        var minDate = today.AddYears(-MinYearsAgo);
        var maxDate = today.AddYears(-MaxYearsAgo);

        if (date < minDate || date > maxDate)
        {
            var message = !string.IsNullOrEmpty(ErrorMessage)
                ? ErrorMessage
                : $"La date doit être entre {minDate:dd/MM/yyyy} et {maxDate:dd/MM/yyyy}";

            return new ValidationResult(message);
        }

        return ValidationResult.Success;
    }
}
