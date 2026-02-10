# 📎 Attached Properties dans WPF

## 🎯 Qu'est-ce qu'une Attached Property ?

Une **Attached Property** est une propriété spéciale qui peut être "attachée" à n'importe quel élément WPF, même si cet élément ne la définit pas nativement.

---

## 🔀 Dependency Property vs Attached Property

| Caractéristique | **Dependency Property** | **Attached Property** |
|----------------|------------------------|----------------------|
| **Définition** | Propriété SUR la classe | Propriété qui s'attache À une classe |
| **Syntaxe** | `element.Property = value` | `Classe.SetProperty(element, value)` |
| **Usage XAML** | `<Button Content="..."/>` | `<Button Grid.Row="0"/>` |
| **Exemple** | `Button.Content`, `TextBox.Text` | `Grid.Row`, `Canvas.Left` |

---

## 📝 Exemples natifs WPF

WPF utilise déjà massivement les Attached Properties :

```xaml
<!-- Grid.Row et Grid.Column sont des Attached Properties -->
<Button Grid.Row="0" Grid.Column="1"/>

<!-- Canvas.Left et Canvas.Top sont des Attached Properties -->
<Rectangle Canvas.Left="50" Canvas.Top="100"/>

<!-- DockPanel.Dock est une Attached Property -->
<Button DockPanel.Dock="Top"/>

<!-- ToolTip.Text est une Attached Property -->
<Button ToolTip.Text="Cliquez-moi"/>
```

---

## 🛠️ Comment créer une Attached Property ?

### 1. Déclaration de la propriété

```csharp
public static class MaClasse
{
    // 1. Déclarer la DependencyProperty (statique)
    public static readonly DependencyProperty MaPropProperty =
        DependencyProperty.RegisterAttached(
            "MaProp",                           // Nom de la propriété
            typeof(string),                     // Type de la propriété
            typeof(MaClasse),                   // Classe propriétaire
            new PropertyMetadata(              // Métadonnées
                "valeur par défaut",
                OnMaPropChanged));              // Callback optionnel

    // 2. Getter statique (REQUIS)
    public static string GetMaProp(DependencyObject obj)
    {
        return (string)obj.GetValue(MaPropProperty);
    }

    // 3. Setter statique (REQUIS)
    public static void SetMaProp(DependencyObject obj, string value)
    {
        obj.SetValue(MaPropProperty, value);
    }

    // 4. Callback optionnel quand la propriété change
    private static void OnMaPropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Réagir au changement
        var oldValue = (string)e.OldValue;
        var newValue = (string)e.NewValue;

        // Appliquer la logique...
    }
}
```

### 2. Utilisation en XAML

```xaml
<Window xmlns:local="clr-namespace:MonNamespace">
    <Button local:MaClasse.MaProp="Ma valeur"/>
</Window>
```

### 3. Utilisation en C#

```csharp
// Setter
MaClasse.SetMaProp(monButton, "Ma valeur");

// Getter
string valeur = MaClasse.GetMaProp(monButton);
```

---

## 💡 Cas d'usage pratiques

### 1. **PlaceholderHelper** (cette démo)
Ajoute un placeholder aux TextBox sans créer un contrôle personnalisé.

```xaml
<TextBox local:PlaceholderHelper.Text="Entrez votre nom..."/>
```

**Avantage** : Évite de créer une classe `TextBoxAvecPlaceholder`.

---

### 2. **BadgeHelper** (cette démo)
Ajoute un badge de notification sur n'importe quel contrôle.

```xaml
<Button local:BadgeHelper.Badge="5" Content="Messages"/>
```

**Avantage** : Fonctionne sur Button, TextBlock, Image, etc.

---

### 3. **CornerRadiusHelper** (cette démo)
Applique des coins arrondis facilement.

```xaml
<Border local:CornerRadiusHelper.Radius="10"/>
```

---

## 🎨 Pourquoi utiliser des Attached Properties ?

### ✅ Avantages

1. **Réutilisabilité** : Une seule implémentation pour tous les contrôles
2. **Séparation des préoccupations** : Ajouter des comportements sans modifier les classes
3. **Composition** : Combiner plusieurs Attached Properties sur un élément
4. **XAML-friendly** : Syntaxe claire et lisible

### ❌ Quand NE PAS les utiliser

- Si la propriété est spécifique à UNE classe → Utilisez une Dependency Property classique
- Si vous avez besoin de logique complexe → Créez un contrôle personnalisé

---

## 🔍 Différences avec les Behaviors

| Caractéristique | **Attached Property** | **Behavior** (Blend SDK) |
|----------------|----------------------|--------------------------|
| **Complexité** | Simple, légère | Plus complexe |
| **Usage** | Propriétés simples | Logique comportementale |
| **Dépendances** | Aucune | NuGet `Microsoft.Xaml.Behaviors` |

**Exemple Behavior** :
```xaml
<TextBox>
    <i:Interaction.Behaviors>
        <behaviors:NumericOnlyBehavior/>
    </i:Interaction.Behaviors>
</TextBox>
```

---

## 📚 Résumé

- **Attached Property** = Propriété qu'on "attache" à n'importe quel élément
- Utilise `RegisterAttached` au lieu de `Register`
- Requiert des getters/setters **statiques**
- Syntaxe XAML : `<Element Classe.Propriété="valeur"/>`
- Cas d'usage : Ajouter des comportements réutilisables sans héritage

---

## 🎓 Points clés à retenir

1. Les Attached Properties sont déclarées avec `RegisterAttached`
2. Elles nécessitent des méthodes `Get` et `Set` statiques
3. On peut les attacher à N'IMPORTE QUEL `DependencyObject`
4. WPF les utilise massivement (`Grid.Row`, `Canvas.Left`, etc.)
5. Parfaites pour ajouter des comportements transversaux

---

## 🧪 Testez dans la démo !

1. **Lancez** `Demo03-AttachedProperty`
2. **Observez** les 3 exemples : Placeholder, Badge, CornerRadius
3. **Modifiez** les valeurs dans le XAML pour voir les changements
4. **Consultez** le code source des helpers pour comprendre l'implémentation
