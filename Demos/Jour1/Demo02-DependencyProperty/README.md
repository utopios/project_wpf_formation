# Demo 02 - Dependency Properties

## Objectif
Créer et utiliser des Dependency Properties personnalisées avec validation, coercion et callbacks.

## Points démontrés
1. Création d'une Dependency Property complète
2. PropertyMetadata et FrameworkPropertyMetadata
3. Callbacks de changement (PropertyChangedCallback)
4. Coercion de valeur (CoerceValueCallback)
5. Validation (ValidateValueCallback)
6. Ordre de résolution des valeurs

## Contrôle créé : NumericUpDown

Un contrôle personnalisé avec :
- `Value` : valeur actuelle
- `Minimum` : valeur minimale
- `Maximum` : valeur maximale
- `Increment` : pas d'incrémentation

## Instructions

1. Examiner le code de `NumericUpDown.cs`
2. Observer l'ordre : Validation → Coercion → Callback
3. Tester les différents scénarios de validation
4. Modifier Min/Max et observer la coercion automatique

## Code clé

```csharp
public static readonly DependencyProperty ValueProperty =
    DependencyProperty.Register(
        nameof(Value),
        typeof(double),
        typeof(NumericUpDown),
        new FrameworkPropertyMetadata(
            0.0,                          // Valeur par défaut
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnValueChanged,               // Callback changement
            CoerceValue),                 // Coercion
        ValidateValue);                   // Validation
```

## Questions à poser aux stagiaires

1. Dans quel ordre sont appelés Validate, Coerce et Changed ?
2. Que se passe-t-il si la validation échoue ?
3. Comment forcer la re-coercion d'une propriété ?
