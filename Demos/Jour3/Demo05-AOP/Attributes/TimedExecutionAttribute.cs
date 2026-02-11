namespace AopDemo.Attributes;

/// <summary>
/// Marqueur pour l'aspect TIMED EXECUTION : mesure le temps d'exécution.
///
/// En C# pur (sans IL weaving comme PostSharp/Fody), on ne peut pas
/// intercepter automatiquement les appels de méthodes.
/// Cet attribut sert de marqueur — l'interception est faite
/// via la méthode helper ExecuteTimed() de la base class.
///
/// Utilisation :
///   [TimedExecution]
///   private async Task LoadData()
///   {
///       await ExecuteTimed(async () =>
///       {
///           // code à mesurer
///       });
///   }
///
/// Note : Avec PostSharp ou Fody.MethodTimer, l'interception serait
/// automatique via IL weaving (modification du bytecode à la compilation).
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TimedExecutionAttribute : Attribute
{
}
