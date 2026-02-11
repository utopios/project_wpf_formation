namespace AopDemo.Attributes;

/// <summary>
/// Aspect DIRTY TRACKING : marque automatiquement l'objet comme modifié
/// quand une propriété décorée change par rapport à sa valeur initiale.
///
/// Cross-cutting concern : le suivi des modifications est déclaratif.
/// Le ViewModel expose IsDirty sans aucune logique manuelle.
///
/// Utilisation :
///   [AutoDirty]
///   public string Name { get; set; }
///
///   // Après modification : IsDirty == true
///   // Si restauré à la valeur initiale : IsDirty == false
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class AutoDirtyAttribute : Attribute
{
}
