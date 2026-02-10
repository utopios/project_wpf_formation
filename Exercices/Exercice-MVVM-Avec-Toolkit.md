# Projet GestionStock - Exercice 4 : Passer au MVVM avec CommunityToolkit.Mvvm

## Contexte
Le projet GestionStock dispose d'une fenetre `ProductDetailView` qui fonctionne en code-behind pur :
- Les controles sont remplis manuellement dans `LoadProductDetails()`
- Le badge de statut est gere dans `UpdateStatusBadge()`
- Le bouton "Modifier" ouvre directement une fenetre depuis le code-behind

L'objectif est de faire evoluer cette vue vers le pattern MVVM **en utilisant CommunityToolkit.Mvvm** (deja installe dans le projet), afin de simplifier le code et d'harmoniser avec les autres ViewModels du projet (`ProductListViewModel`, `DashboardViewModel`).

## Prerequis
- Avoir realise l'exercice 3 (MVVM sans toolkit) pour comprendre les mecanismes sous-jacents
- Le package `CommunityToolkit.Mvvm 8.2.2` est deja reference dans le `.csproj`

---

## Etat actuel du projet

### Fichiers concernes
| Fichier | Role actuel |
|---------|-------------|
| `Views/ProductDetailView.xaml` | Interface d'affichage avec controles nommes (`x:Name`) |
| `Views/ProductDetailView.xaml.cs` | ~100 lignes : `LoadProductDetails()`, `UpdateStatusBadge()`, handlers Click |

### Ce qui existe deja dans le projet avec le Toolkit
| Fichier | A observer |
|---------|-----------|
| `ViewModels/ProductListViewModel.cs` | Utilisation de `[ObservableProperty]`, `[RelayCommand]`, `[NotifyCanExecuteChangedFor]` |
| `ViewModels/ViewModelBase.cs` | Classe de base avec `ObservableValidator`, `IsBusy`, `ExecuteAsync()` |
| `ViewModels/DashboardViewModel.cs` | Autre exemple de ViewModel avec le toolkit |

---

## Travail demande

### Etape 1 - Analyser le code existant du Toolkit

Avant de coder, ouvrir `ViewModels/ProductListViewModel.cs` et repondre aux questions :

1. **`[ObservableProperty]`** sur `private string _searchText` :
   - Quelle propriete publique est generee automatiquement ?
   - Pourquoi la classe doit-elle etre `partial` ?

2. **`[RelayCommand]`** sur `private async Task LoadDataAsync()` :
   - Quelle propriete de commande est generee ?
   - Quel est le nom de convention ? (methode `XxxAsync` -> commande `XxxCommand`)

3. **`[NotifyPropertyChangedFor(nameof(HasSelectedProduct))]`** sur `_selectedProduct` :
   - Que se passe-t-il quand `SelectedProduct` change ?
   - Pourquoi c'est utile pour une propriete calculee comme `HasSelectedProduct` ?

4. **`[NotifyCanExecuteChangedFor(nameof(EditProductCommand))]`** sur `_selectedProduct` :
   - Quel est l'effet sur le bouton "Modifier" quand `SelectedProduct` devient `null` ?

---

### Etape 2 - Creer le ViewModel `ProductInfoViewModel`

**Fichier a creer** : `ViewModels/ProductInfoViewModel.cs`

Ce ViewModel reprend la logique de `ProductDetailView.xaml.cs` en utilisant le toolkit.

#### Heritage
```csharp
public partial class ProductInfoViewModel : ObservableObject
```

#### Proprietes observables
Utiliser `[ObservableProperty]` pour toutes les donnees affichees :

```csharp
[ObservableProperty]
private string _productName = string.Empty;

[ObservableProperty]
private string _productCode = string.Empty;

[ObservableProperty]
private string _categoryName = string.Empty;

[ObservableProperty]
private string _supplierName = string.Empty;

[ObservableProperty]
private string _priceDisplay = string.Empty;

[ObservableProperty]
private int _quantityInStock;

[ObservableProperty]
private int _minimumStock;

[ObservableProperty]
private string _totalValueDisplay = string.Empty;

[ObservableProperty]
private string _activeStatus = string.Empty;

[ObservableProperty]
private string _description = string.Empty;

[ObservableProperty]
private string _createdAtDisplay = string.Empty;

[ObservableProperty]
private string _updatedAtDisplay = string.Empty;

[ObservableProperty]
private string _statusText = string.Empty;

[ObservableProperty]
private string _statusColor = string.Empty;
```

#### Constructeur
Le constructeur recoit un `Product` et remplit toutes les proprietes :
- Formater le prix avec `{product.UnitPrice:C}`
- Formater les dates avec `ToString("dd/MM/yyyy HH:mm")`
- Calculer le statut : "Rupture de stock" / "Stock faible" / "En stock"
- Calculer la couleur : `"#F44336"` / `"#FF9800"` / `"#4CAF50"`

#### Commandes
Utiliser `[RelayCommand]` :
- `Edit()` : ouvre `ProductEditView` avec le produit
- `CloseWindow()` : demande la fermeture de la fenetre

#### Evenement de fermeture
```csharp
public event Action<bool>? CloseRequested;
```

---

### Etape 3 - Migrer la vue `ProductDetailView`

**Fichiers a modifier** :
- `Views/ProductDetailView.xaml`
- `Views/ProductDetailView.xaml.cs`

#### Remplacements dans le XAML

| Avant (x:Name) | Apres (Binding) |
|----------------|-----------------|
| `x:Name="ProductNameText"` | `Text="{Binding ProductName}"` |
| `x:Name="ProductCodeText"` | `Text="{Binding ProductCode}"` |
| `x:Name="CategoryText"` | `Text="{Binding CategoryName}"` |
| `x:Name="SupplierText"` | `Text="{Binding SupplierName}"` |
| `x:Name="PriceText"` | `Text="{Binding PriceDisplay}"` |
| `x:Name="QuantityText"` | `Text="{Binding QuantityInStock}"` |
| `x:Name="MinStockText"` | `Text="{Binding MinimumStock}"` |
| `x:Name="TotalValueText"` | `Text="{Binding TotalValueDisplay}"` |
| `x:Name="ActiveText"` | `Text="{Binding ActiveStatus}"` |
| `x:Name="DescriptionText"` | `Text="{Binding Description}"` |
| `x:Name="CreatedAtText"` | `Text="{Binding CreatedAtDisplay}"` |
| `x:Name="UpdatedAtText"` | `Text="{Binding UpdatedAtDisplay}"` |
| `x:Name="StatusText"` | `Text="{Binding StatusText}"` |
| `Click="EditButton_Click"` | `Command="{Binding EditCommand}"` |
| `Click="CloseButton_Click"` | `Command="{Binding CloseWindowCommand}"` |

#### Badge de statut
Pour binder la couleur du badge, creer un converter `StringToBrushConverter` dans `Resources/Converters/ValueConverters.cs` :
```csharp
public class StringToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string colorString)
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorString));
        return Brushes.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
```

Puis dans le XAML :
```xml
<Border Background="{Binding StatusColor, Converter={StaticResource StringToBrushConverter}}">
    <TextBlock Text="{Binding StatusText}" ... />
</Border>
```

#### Code-behind final
```csharp
public partial class ProductDetailView : Window
{
    public ProductDetailView(Product product)
    {
        InitializeComponent();
        var viewModel = new ProductInfoViewModel(product);
        viewModel.CloseRequested += result =>
        {
            DialogResult = result;
            Close();
        };
        DataContext = viewModel;
    }
}
```

---

### Etape 4 (Bonus) - Proprietes calculees avec `[NotifyPropertyChangedFor]`

Ameliorer le ViewModel pour que le statut se recalcule automatiquement si les quantites changent :

```csharp
[ObservableProperty]
[NotifyPropertyChangedFor(nameof(StockStatusText))]
[NotifyPropertyChangedFor(nameof(StockStatusColor))]
private int _quantityInStock;

[ObservableProperty]
[NotifyPropertyChangedFor(nameof(StockStatusText))]
[NotifyPropertyChangedFor(nameof(StockStatusColor))]
private int _minimumStock;

// Proprietes calculees (pas de [ObservableProperty], ce sont des getters)
public string StockStatusText => QuantityInStock == 0
    ? "Rupture de stock"
    : QuantityInStock <= MinimumStock
        ? "Stock faible"
        : "En stock";

public string StockStatusColor => QuantityInStock == 0
    ? "#F44336"
    : QuantityInStock <= MinimumStock
        ? "#FF9800"
        : "#4CAF50";
```

**Avantage** : Si un mouvement de stock modifie `QuantityInStock`, le badge se met a jour automatiquement.

---

## Rappel : Comparaison Sans Toolkit vs Avec Toolkit

| Concept | Exercice 3 (sans toolkit) | Exercice 4 (avec toolkit) |
|---------|--------------------------|--------------------------|
| Notification | `BindableBase` + `SetProperty()` manuels | `[ObservableProperty]` genere tout |
| Commandes | `DelegateCommand` manuelle | `[RelayCommand]` genere tout |
| Proprietes calculees | `OnPropertyChanged("Prop")` a appeler soi-meme | `[NotifyPropertyChangedFor]` automatique |
| Classe | Normale | `partial` (obligatoire pour la generation) |
| Volume de code | ~100 lignes de base | ~5 lignes (le toolkit genere le reste) |

---

## Contraintes
- **Obligatoire** : Utiliser `CommunityToolkit.Mvvm` pour les proprietes et commandes
- **Interdit** : Manipuler les controles par `x:Name` dans le code-behind
- **Ne pas modifier** les ViewModels existants (`ProductDetailViewModel.cs`, `ProductListViewModel.cs`)
- Le nouveau ViewModel s'appelle **`ProductInfoViewModel`** (pour ne pas entrer en conflit avec `ProductDetailViewModel`)

## Criteres de validation
- [ ] `ProductInfoViewModel` herite de `ObservableObject`
- [ ] Toutes les proprietes utilisent `[ObservableProperty]`
- [ ] Les commandes utilisent `[RelayCommand]`
- [ ] Le XAML n'a plus aucun `x:Name` pour les donnees (seuls les noms structurels sont acceptes)
- [ ] Le code-behind de `ProductDetailView.xaml.cs` fait moins de 15 lignes
- [ ] Le bouton "Modifier" ouvre la fenetre d'edition
- [ ] Le badge de statut affiche la bonne couleur et le bon texte
- [ ] L'application GestionStock compile et fonctionne sans regression
