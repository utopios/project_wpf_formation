# Projet GestionStock - Exercice 5 : Gestion des Fournisseurs avec CommunityToolkit.Mvvm

## Contexte
Le projet GestionStock gere les produits, les categories et les mouvements de stock, mais la gestion des fournisseurs n'est pas encore accessible depuis l'interface utilisateur. Le modele `Supplier` existe deja dans `Models/Product.cs`, et l'interface `ISupplierRepository` est definie dans `Services/Interfaces.cs`.

L'objectif est de creer un module complet de gestion des fournisseurs (liste, ajout, modification, suppression) **en utilisant CommunityToolkit.Mvvm**, en s'inspirant des patterns deja en place dans `ProductListViewModel`.

## Prerequis
- Avoir realise les exercices 3 et 4 pour maitriser les patterns MVVM
- Le package `CommunityToolkit.Mvvm 8.2.2` est deja reference dans le `.csproj`

---

## Rappel : le modele `Supplier`

```csharp
public class Supplier
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom du fournisseur est requis")]
    [StringLength(100)]
    public required string Name { get; set; }

    [StringLength(100)]
    public string? ContactName { get; set; }

    [EmailAddress(ErrorMessage = "Email invalide")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Numero de telephone invalide")]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
```

## Rappel : l'interface `ISupplierRepository`

```csharp
public interface ISupplierRepository : IRepository<Supplier>
{
    Task<IEnumerable<Supplier>> GetActiveAsync();
}
```

Herite de `IRepository<T>` qui fournit : `GetAllAsync()`, `GetByIdAsync(int)`, `AddAsync(T)`, `UpdateAsync(T)`, `DeleteAsync(int)`.

---

## Etat actuel du projet

### Ce qui existe deja
| Element | Emplacement |
|---------|-------------|
| Modele `Supplier` | `Models/Product.cs` |
| Interface `ISupplierRepository` | `Services/Interfaces.cs` |
| Classe de base `ViewModelBase` | `ViewModels/ViewModelBase.cs` (avec `IsBusy`, `ExecuteAsync()`) |
| Classe de base `NavigableViewModelBase` | `ViewModels/ViewModelBase.cs` (avec `OnNavigatedTo()`) |
| Exemple complet de liste | `ViewModels/ProductListViewModel.cs` |
| Exemple complet de formulaire | `ViewModels/ProductEditFormViewModel.cs` |

### Ce qu'il faut creer
| Fichier | Role |
|---------|------|
| `ViewModels/SupplierListViewModel.cs` | ViewModel pour la liste des fournisseurs |
| `ViewModels/SupplierEditViewModel.cs` | ViewModel pour le formulaire d'ajout/modification |
| `Views/SupplierListView.xaml` + `.cs` | Vue liste |
| `Views/SupplierEditView.xaml` + `.cs` | Vue formulaire ) |

---

## Travail demande

### Etape 1 - Implementer le `SupplierRepository`

**Fichier a creer** : `Services/SupplierRepository.cs`

En s'inspirant de `ProductRepository.cs`, implementer un repository pour les fournisseurs :

```csharp
public class SupplierRepository : ISupplierRepository
```

#### Methodes a implementer
| Methode | Comportement |
|---------|-------------|
| `GetAllAsync()` | Retourne tous les fournisseurs, tries par nom, avec le `Include(s => s.Products)` pour charger les produits associes |
| `GetByIdAsync(int id)` | Retourne un fournisseur par son Id avec ses produits |
| `AddAsync(Supplier)` | Ajoute un fournisseur en base |
| `UpdateAsync(Supplier)` | Met a jour un fournisseur existant |
| `DeleteAsync(int id)` | Supprime un fournisseur par son Id |
| `GetActiveAsync()` | Retourne uniquement les fournisseurs avec `IsActive == true` |

#### Enregistrement dans le conteneur DI
Dans `App.xaml.cs`, ajouter l'enregistrement du repository :
```csharp
services.AddScoped<ISupplierRepository, SupplierRepository>();
```

---

### Etape 2 - Creer le ViewModel `SupplierListViewModel`


S'inspirer directement de `ProductListViewModel` pour la structure.


---

### Etape 3 - Creer le ViewModel `SupplierEditViewModel`

**Fichier a creer** : `ViewModels/SupplierEditViewModel.cs`
