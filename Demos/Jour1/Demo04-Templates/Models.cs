namespace TemplatesDemo;

/// <summary>
/// Modèle de produit pour démontrer le DataTemplate
/// </summary>
public class Product
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public bool IsLowStock => Stock > 0 && Stock <= 5;
    public bool IsOutOfStock => Stock == 0;

    public string StockStatus => Stock switch
    {
        0 => "Épuisé",
        <= 5 => $"{Stock} restants",
        _ => "En stock"
    };
}

/// <summary>
/// Modèle d'utilisateur pour démontrer le DataTemplate
/// </summary>
public class User
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public string FullName => $"{FirstName} {LastName}";

    public string Initials => string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName)
        ? "?"
        : $"{FirstName[0]}{LastName[0]}".ToUpper();
}
