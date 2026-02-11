using System.ComponentModel.DataAnnotations;

namespace CustomAnnotationsDemo.Annotations;

/// <summary>
/// Annotation avec liste de mots interdits (params)
/// Vérifie qu'une chaîne ne contient aucun mot interdit
///
/// Utilisation: [ContainsNo("admin", "root", "test")]
/// </summary>
public class ContainsNoAttribute : ValidationAttribute
{
    private readonly string[] _forbiddenWords;

    public ContainsNoAttribute(params string[] forbiddenWords)
        : base("La valeur contient un mot interdit : {0}")
    {
        _forbiddenWords = forbiddenWords;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string text || string.IsNullOrWhiteSpace(text))
            return ValidationResult.Success;

        var lowerText = text.ToLowerInvariant();

        foreach (var word in _forbiddenWords)
        {
            if (lowerText.Contains(word.ToLowerInvariant()))
            {
                return new ValidationResult(
                    string.Format(ErrorMessageString, word));
            }
        }

        return ValidationResult.Success;
    }
}
