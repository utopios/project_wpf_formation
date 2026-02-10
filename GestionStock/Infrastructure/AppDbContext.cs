namespace GestionStock.Infrastructure;

/// <summary>
/// Contexte Entity Framework pour l'accès aux données
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => p.Code).IsUnique();

            entity.Property(p => p.Code).HasMaxLength(20).IsRequired();
            entity.Property(p => p.Name).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Description).HasMaxLength(500);
            entity.Property(p => p.UnitPrice).HasPrecision(18, 2);

            entity.HasOne(p => p.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(p => p.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Supplier)
                  .WithMany(s => s.Products)
                  .HasForeignKey(p => p.SupplierId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuration Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => c.Name).IsUnique();
            entity.Property(c => c.Name).HasMaxLength(50).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(200);
        });

        // Configuration Supplier
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).HasMaxLength(100).IsRequired();
            entity.Property(s => s.ContactName).HasMaxLength(100);
            entity.Property(s => s.Email).HasMaxLength(100);
            entity.Property(s => s.Phone).HasMaxLength(20);
            entity.Property(s => s.Address).HasMaxLength(200);
        });

        // Configuration StockMovement
        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Reason).HasMaxLength(500);
            entity.Property(m => m.Reference).HasMaxLength(50);
            entity.Property(m => m.CreatedBy).HasMaxLength(100);

            entity.HasOne(m => m.Product)
                  .WithMany(p => p.StockMovements)
                  .HasForeignKey(m => m.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed Data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Catégories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Électronique", Description = "Composants et appareils électroniques" },
            new Category { Id = 2, Name = "Bureautique", Description = "Fournitures de bureau" },
            new Category { Id = 3, Name = "Informatique", Description = "Matériel informatique" },
            new Category { Id = 4, Name = "Outillage", Description = "Outils et équipements" }
        );

        // Fournisseurs
        modelBuilder.Entity<Supplier>().HasData(
            new Supplier { Id = 1, Name = "TechSupply SA", ContactName = "Jean Martin", Email = "contact@techsupply.fr", Phone = "01 23 45 67 89" },
            new Supplier { Id = 2, Name = "Bureau Plus", ContactName = "Marie Dupont", Email = "info@bureauplus.fr", Phone = "01 98 76 54 32" },
            new Supplier { Id = 3, Name = "Global Parts", ContactName = "Pierre Durand", Email = "sales@globalparts.com", Phone = "01 11 22 33 44" }
        );

        // Produits
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Code = "ELEC-001", Name = "Câble HDMI 2m", Description = "Câble HDMI haute vitesse 2 mètres", UnitPrice = 12.99m, QuantityInStock = 150, MinimumStock = 20, CategoryId = 1, SupplierId = 1, CreatedAt = DateTime.Now },
            new Product { Id = 2, Code = "ELEC-002", Name = "Adaptateur USB-C", Description = "Hub USB-C multiport", UnitPrice = 29.99m, QuantityInStock = 75, MinimumStock = 15, CategoryId = 1, SupplierId = 1, CreatedAt = DateTime.Now },
            new Product { Id = 3, Code = "BUR-001", Name = "Ramette papier A4", Description = "Papier blanc 80g, 500 feuilles", UnitPrice = 5.49m, QuantityInStock = 200, MinimumStock = 50, CategoryId = 2, SupplierId = 2, CreatedAt = DateTime.Now },
            new Product { Id = 4, Code = "BUR-002", Name = "Stylos bille x10", Description = "Pack de 10 stylos bille bleus", UnitPrice = 3.99m, QuantityInStock = 8, MinimumStock = 20, CategoryId = 2, SupplierId = 2, CreatedAt = DateTime.Now },
            new Product { Id = 5, Code = "INFO-001", Name = "Souris sans fil", Description = "Souris optique sans fil", UnitPrice = 19.99m, QuantityInStock = 45, MinimumStock = 10, CategoryId = 3, SupplierId = 1, CreatedAt = DateTime.Now },
            new Product { Id = 6, Code = "INFO-002", Name = "Clavier mécanique", Description = "Clavier mécanique RGB", UnitPrice = 79.99m, QuantityInStock = 0, MinimumStock = 5, CategoryId = 3, SupplierId = 3, CreatedAt = DateTime.Now },
            new Product { Id = 7, Code = "OUT-001", Name = "Tournevis précision", Description = "Kit 24 tournevis de précision", UnitPrice = 15.99m, QuantityInStock = 30, MinimumStock = 10, CategoryId = 4, SupplierId = 3, CreatedAt = DateTime.Now }
        );
    }
}
