namespace AopDemo.Attributes;

/// <summary>
/// Aspect NOTIFICATION EN CASCADE : quand cette propriété change,
/// notifie automatiquement d'autres propriétés calculées.
///
/// Cross-cutting concern : les dépendances entre propriétés sont déclaratives.
///
/// Utilisation :
///   [DependsOn(nameof(FullName), nameof(IsValid))]
///   public string FirstName { get; set; }
///
///   // Modifier FirstName → PropertyChanged déclenché aussi pour FullName et IsValid
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class DependsOnAttribute : Attribute
{
    public string[] PropertyNames { get; }

    public DependsOnAttribute(params string[] propertyNames)
    {
        PropertyNames = propertyNames;
    }
}
