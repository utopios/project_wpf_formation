namespace AopDemo.Attributes;

/// <summary>
/// Aspect LOG : journalise chaque changement de propriété
/// avec le timestamp et la nouvelle valeur.
///
/// Cross-cutting concern : le logging est extrait du code métier
/// et appliqué de manière déclarative.
///
/// Utilisation :
///   [LogOnChange]
///   public string Name { get; set; }
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class LogOnChangeAttribute : Attribute
{
}
