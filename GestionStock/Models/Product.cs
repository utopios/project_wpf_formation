namespace GestionStock.Models;

/// <summary>
/// Représente un produit dans le système de gestion de stock
/// </summary>
public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le code produit est requis")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Le code doit contenir entre 3 et 20 caractères")]
    public required string Code { get; set; }

    [Required(ErrorMessage = "Le nom du produit est requis")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 100 caractères")]
    public required string Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être positif")]
    public decimal UnitPrice { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "La quantité doit être positive")]
    public int QuantityInStock { get; set; }

    [Range(0, int.MaxValue)]
    public int MinimumStock { get; set; } = 10;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;

    // Propriétés calculées
    public bool IsLowStock => QuantityInStock <= MinimumStock;
    public bool IsOutOfStock => QuantityInStock == 0;
    public decimal TotalValue => UnitPrice * QuantityInStock;

    // Navigation
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}

/// <summary>
/// Catégorie de produits
/// </summary>
public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom de la catégorie est requis")]
    [StringLength(50)]
    public required string Name { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    public string? IconPath { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();

    public int ProductCount => Products.Count;
}

/// <summary>
/// Fournisseur
/// </summary>
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

    [Phone(ErrorMessage = "Numéro de téléphone invalide")]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

/// <summary>
/// Mouvement de stock (entrée/sortie)
/// </summary>
public class StockMovement
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public MovementType Type { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être supérieure à 0")]
    public int Quantity { get; set; }

    public int QuantityBefore { get; set; }
    public int QuantityAfter { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }

    public string? Reference { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? CreatedBy { get; set; }
}

/// <summary>
/// Type de mouvement de stock
/// </summary>
public enum MovementType
{
    [Display(Name = "Entrée")]
    StockIn,

    [Display(Name = "Sortie")]
    StockOut,

    [Display(Name = "Ajustement")]
    Adjustment,

    [Display(Name = "Retour")]
    Return,

    [Display(Name = "Inventaire")]
    Inventory
}
