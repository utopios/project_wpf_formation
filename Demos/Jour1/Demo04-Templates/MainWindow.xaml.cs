using System.Windows;

namespace TemplatesDemo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoadSampleData();
    }

    private void LoadSampleData()
    {
        // Données de produits pour la démo
        var products = new List<Product>
        {
            new() { Name = "Ordinateur Portable", Category = "Électronique", Price = 899.99m, Stock = 15 },
            new() { Name = "Souris Sans Fil", Category = "Accessoires", Price = 29.99m, Stock = 3 },
            new() { Name = "Clavier Mécanique", Category = "Accessoires", Price = 149.99m, Stock = 0 },
            new() { Name = "Écran 27\"", Category = "Électronique", Price = 349.99m, Stock = 8 }
        };

        ProductsItemsControl.ItemsSource = products;

        // Données d'utilisateurs pour la démo
        var users = new List<User>
        {
            new() { FirstName = "Marie", LastName = "Dupont", Email = "marie.dupont@email.com", IsActive = true },
            new() { FirstName = "Jean", LastName = "Martin", Email = "jean.martin@email.com", IsActive = true },
            new() { FirstName = "Sophie", LastName = "Bernard", Email = "sophie.bernard@email.com", IsActive = false },
            new() { FirstName = "Pierre", LastName = "Dubois", Email = "pierre.dubois@email.com", IsActive = true },
            new() { FirstName = "Claire", LastName = "Moreau", Email = "claire.moreau@email.com", IsActive = false }
        };

        UsersListBox.ItemsSource = users;
    }
}
