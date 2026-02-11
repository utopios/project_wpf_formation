namespace AopDemo.Attributes;

/// <summary>
/// Aspect CLAMP : force une valeur numérique dans une plage [Min, Max].
/// Si la valeur dépasse, elle est automatiquement corrigée.
///
/// Cross-cutting concern : la contrainte de plage est déclarative,
/// pas besoin de if/else dans le setter.
///
/// Utilisation :
///   [Clamp(0, 100)]
///   public int Percentage { get; set; }
///
///   // Percentage = 150 → automatiquement corrigé à 100
///   // Percentage = -5  → automatiquement corrigé à 0
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ClampAttribute : Attribute
{
    public double Minimum { get; }
    public double Maximum { get; }

    public ClampAttribute(double minimum, double maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
    }
}
