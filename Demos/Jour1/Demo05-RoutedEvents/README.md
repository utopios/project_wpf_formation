# 📖 Comprendre les Routed Events dans WPF

## 🎯 Qu'est-ce qu'un Routed Event ?

Un **Routed Event** est un événement qui peut parcourir l'arbre visuel WPF dans un ordre spécifique.
Contrairement aux événements .NET classiques, ils ne s'arrêtent pas à l'élément source mais "voyagent" à travers la hiérarchie.

---

## 🔀 Les 3 stratégies de routage

### 1. 🔵 **Bubbling** (Remontée)
- **Direction** : De l'élément cible → vers la racine (parent, grand-parent, etc.)
- **Exemples** : `MouseDown`, `Click`, `KeyDown`
- **Quand l'utiliser** : C'est le comportement par défaut. Utile quand un parent veut réagir aux actions de ses enfants.

```
👆 Click sur Button
   ⬇️
Button → InnerBorder → MiddleBorder → OuterBorder → Window
```

**Exemple concret** :
Quand vous cliquez sur un bouton dans un formulaire, le clic "remonte" jusqu'au formulaire parent, qui peut décider de réagir.

---

### 2. 🟣 **Tunneling** (Descente)
- **Direction** : De la racine → vers l'élément cible
- **Préfixe** : `Preview*` (ex: `PreviewMouseDown`, `PreviewKeyDown`)
- **Quand l'utiliser** : Pour intercepter un événement AVANT qu'il n'atteigne la cible.

```
👆 Click sur Button
   ⬇️
Window → OuterBorder → MiddleBorder → InnerBorder → Button
```

**Exemple concret** :
Vous pouvez valider une saisie AVANT que la TextBox ne reçoive le caractère tapé.

---

### 3. 🟢 **Direct** (Direct)
- **Direction** : Uniquement sur l'élément cible
- **Exemples** : `MouseEnter`, `MouseLeave`, `Loaded`
- **Quand l'utiliser** : Quand seul l'élément cible doit réagir.

```
👆 MouseEnter sur Button
   ⬇️
Seulement Button (ne remonte ni ne descend)
```

---

## 🔄 Ordre complet d'exécution

Quand vous cliquez sur le bouton dans la démo, voici l'ordre :

```
1. [TUNNEL] Window.PreviewMouseDown
2. [TUNNEL] OuterBorder.PreviewMouseDown
3. [TUNNEL] MiddleBorder.PreviewMouseDown
4. [TUNNEL] InnerBorder.PreviewMouseDown
5. [TUNNEL] Button.PreviewMouseDown
   ⬆️ Descente terminée, début de la remontée ⬆️
6. [BUBBLE] Button.Click
7. [BUBBLE] InnerBorder.MouseDown
8. [BUBBLE] MiddleBorder.MouseDown
9. [BUBBLE] OuterBorder.MouseDown
10. [BUBBLE] Window.MouseDown
```

---

## 🛑 Arrêter la propagation avec `e.Handled`

Vous pouvez arrêter le routage en mettant `e.Handled = true` :

```csharp
private void MiddleBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
{
    // L'événement ne descendra PAS plus bas
    e.Handled = true;
}
```

**⚠️ Important** : `e.Handled` n'arrête pas complètement l'événement, mais empêche sa propagation normale.

---

## 💡 Cas d'usage pratiques

### 1. **Validation globale de saisie** (Tunneling)
```csharp
// Sur le Window, intercepter TOUTES les saisies avant qu'elles n'atteignent les TextBox
private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
{
    // Bloquer les caractères non numériques pour TOUTES les TextBox numériques
    if (!char.IsDigit(e.Text[0]))
    {
        e.Handled = true; // Empêche la saisie
    }
}
```

### 2. **Gestion centralisée des clics** (Bubbling)
```csharp
// Sur un Panel parent, réagir aux clics de TOUS les boutons enfants
private void Panel_MouseDown(object sender, MouseButtonEventArgs e)
{
    if (e.Source is Button button)
    {
        MessageBox.Show($"Vous avez cliqué sur {button.Content}");
    }
}
```

### 3. **Événement localisé** (Direct)
```csharp
// Seulement quand la souris entre sur CE bouton
private void Button_MouseEnter(object sender, MouseEventArgs e)
{
    (sender as Button).Background = Brushes.LightBlue;
}
```

---

## 🧪 Testez dans la démo !

1. **Lancez** `Demo05-RoutedEvents`
2. **Cliquez** sur le bouton bleu "Cliquez-moi !"
3. **Observez** le journal à droite pour voir l'ordre des événements
4. **Cochez** les cases pour arrêter le Tunneling ou Bubbling au niveau du Border Milieu

---

## 📝 Résumé visuel

```
         🏠 Window (racine)
              ⬇️
         📦 OuterBorder
              ⬇️
         📦 MiddleBorder
              ⬇️
         📦 InnerBorder
              ⬇️
         🔘 Button (cible)

🟣 PreviewMouseDown : 🏠 → 📦 → 📦 → 📦 → 🔘 (Tunneling)
🔵 MouseDown        : 🔘 → 📦 → 📦 → 📦 → 🏠 (Bubbling)
🟢 MouseEnter       : 🔘 (Direct, ne bouge pas)
```

---

## 🎓 Pourquoi c'est utile ?

- **Séparation des préoccupations** : Un parent peut gérer les événements de tous ses enfants
- **Interception préventive** : Valider avant que l'événement n'atteigne la cible
- **Flexibilité** : Choisir où et quand réagir dans la hiérarchie

---

## 🔗 Relation avec Dependency Properties

- **Dependency Properties** : Système de propriétés avec héritage et liaison
- **Routed Events** : Système d'événements avec routage dans l'arbre visuel

Les deux utilisent l'arbre visuel WPF mais pour des buts différents :
- **Properties** : Partager des DONNÉES (styles, bindings)
- **Events** : Communiquer des ACTIONS (clics, saisies)
