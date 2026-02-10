# Projet GestionStock - Exercice 3 : Passer au MVVM sans Toolkit

## Contexte
Le projet GestionStock dispose actuellement de deux fenetres fonctionnelles en code-behind pur :
- `ProductDetailView` : affiche les details d'un produit
- `ProductEditView` : permet de creer et modifier un produit

Ces vues accedent directement aux repositories via `App.Services` et manipulent les controles par leur `x:Name`.
L'objectif est de faire evoluer `ProductEditView` vers le pattern MVVM **en implementant vous-meme les mecanismes de base**, sans aucun toolkit.

---

## Etat actuel du projet

### Fichiers concernes
| Fichier | Role actuel |
|---------|-------------|
| `Views/ProductEditView.xaml` | Formulaire avec controles nommes (`x:Name`) |
| `Views/ProductEditView.xaml.cs` | ~270 lignes de code-behind (validation, sauvegarde, chargement) |

### Problemes identifies
1. La logique metier (validation, sauvegarde) est melangee avec l'affichage
2. Impossible de tester la logique sans lancer l'application
3. Le code-behind grossit a chaque nouvelle fonctionnalite

---

## Travail demande

### Etape 1 - Creer la classe de base `BindableBase`

**Fichier a creer** : `ViewModels/BindableBase.cs`

Creer une classe abstraite qui implemente `INotifyPropertyChanged` :

1. Implementer l'evenement `PropertyChanged`
2. Creer une methode protegee `OnPropertyChanged(string propertyName)` qui declenche l'evenement
3. Creer une methode helper `SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")` qui :
   - Compare l'ancienne et la nouvelle valeur
   - Si differentes : met a jour le champ, appelle `OnPropertyChanged`, retourne `true`
   - Si identiques : retourne `false`


---

### Etape 2 - Creer la commande `DelegateCommand`

**Fichier a creer** : `ViewModels/DelegateCommand.cs`

Creer une implementation de `System.Windows.Input.ICommand` :

1. Le constructeur prend :
   - `Action<object?> execute` : l'action a executer
   - `Func<object?, bool>? canExecute` (optionnel) : la condition d'activation du bouton
2. Implementer `Execute(object? parameter)` : appelle l'action
3. Implementer `CanExecute(object? parameter)` : retourne `true` si `canExecute` est null, sinon evalue la condition
4. Lier l'evenement `CanExecuteChanged` au `CommandManager.RequerySuggested` de WPF


---

### Etape 3 - Creer le ViewModel `ProductEditFormViewModel`

**Fichier a creer** : `ViewModels/ProductEditFormViewModel.cs`

Ce ViewModel doit reprendre toute la logique actuellement dans `ProductEditView.xaml.cs`.

#### Constructeur
- Accepte un `Product?` en parametre (`null` = creation, non-null = modification)
- Recupere les repositories via `App.Services`
- Initialise les commandes `SaveCommand` et `CancelCommand`
- Lance le chargement des categories et fournisseurs

#### Proprietes avec notification (heritent de `BindableBase`)
Chaque propriete du formulaire doit utiliser `SetProperty` pour notifier l'UI :

| Propriete | Type | Correspond au controle |
|-----------|------|----------------------|
| `Code` | `string` | `CodeTextBox` |
| `Name` | `string` | `NameTextBox` |
| `Description` | `string?` | `DescriptionTextBox` |
| `UnitPrice` | `decimal` | `PriceTextBox` |
| `QuantityInStock` | `int` | `QuantityTextBox` |
| `MinimumStock` | `int` | `MinStockTextBox` |
| `SelectedCategory` | `Category?` | `CategoryComboBox` |
| `SelectedSupplier` | `Supplier?` | `SupplierComboBox` |
| `IsActive` | `bool` | `IsActiveCheckBox` |
| `Categories` | `IEnumerable<Category>` | Liste deroulante categories |
| `Suppliers` | `IEnumerable<Supplier>` | Liste deroulante fournisseurs |
| `IsLoading` | `bool` | Overlay de chargement |
| `Title` | `string` | Titre de la fenetre |
| `CodeError` | `string?` | Message d'erreur du code |
| `NameError` | `string?` | Message d'erreur du nom |
| `CategoryError` | `string?` | Message d'erreur categorie |
| `PriceError` | `string?` | Message d'erreur prix |
| `QuantityError` | `string?` | Message d'erreur quantite |

#### Commandes
| Commande | Action | Condition |
|----------|--------|-----------|
| `SaveCommand` | Valide puis sauvegarde le produit | Formulaire valide |
| `CancelCommand` | Ferme la fenetre | Toujours actif |

#### Fermeture de la fenetre
Le ViewModel ne connait pas la Window. Pour demander la fermeture, exposer un evenement :
```csharp
public event Action<bool>? CloseRequested;
// true = sauvegarde reussie, false = annulation
```

---

### Etape 4 - Migrer la vue `ProductEditView` vers le Binding

**Fichiers a modifier** :
- `Views/ProductEditView.xaml`
- `Views/ProductEditView.xaml.cs`

#### Dans le XAML
Remplacer les `x:Name` et `Click` par des Bindings 

---

## Contraintes
- **Interdit** : Utiliser `CommunityToolkit.Mvvm` (`[ObservableProperty]`, `[RelayCommand]`, `ObservableObject`)
- **Autorise** : `App.Services.GetRequiredService<T>()` pour acceder aux repositories

