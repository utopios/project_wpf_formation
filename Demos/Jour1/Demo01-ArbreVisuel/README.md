# Demo 01 - Arbre Visuel et Arbre Logique

## Objectif
Comprendre la différence entre l'arbre logique et l'arbre visuel en WPF.

## Points démontrés
1. Navigation dans l'arbre logique avec `LogicalTreeHelper`
2. Navigation dans l'arbre visuel avec `VisualTreeHelper`
3. Différence de profondeur entre les deux arbres
4. Impact des templates sur l'arbre visuel

## Instructions

1. Ouvrir le projet dans Visual Studio
2. Exécuter l'application
3. Cliquer sur "Afficher Arbre Logique" et observer la structure
4. Cliquer sur "Afficher Arbre Visuel" et comparer
5. Noter que l'arbre visuel contient beaucoup plus d'éléments

## Code clé

```csharp
// Navigation dans l'arbre logique
foreach (var child in LogicalTreeHelper.GetChildren(element))
{
    // Traiter l'enfant
}

// Navigation dans l'arbre visuel
int count = VisualTreeHelper.GetChildrenCount(element);
for (int i = 0; i < count; i++)
{
    var child = VisualTreeHelper.GetChild(element, i);
    // Traiter l'enfant
}
```

## Questions à poser aux stagiaires

1. Pourquoi l'arbre visuel a-t-il plus d'éléments ?
2. Quand utiliser l'un plutôt que l'autre ?
3. Comment un template affecte-t-il ces arbres ?
