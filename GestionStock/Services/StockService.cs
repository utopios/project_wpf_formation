namespace GestionStock.Services;

/// <summary>
/// Service de gestion des mouvements de stock
/// </summary>
public class StockService : IStockService
{
    private readonly AppDbContext _context;

    public StockService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StockMovement> AddStockAsync(int productId, int quantity, string? reason = null, string? reference = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("La quantité doit être positive", nameof(quantity));

        var product = await _context.Products.FindAsync(productId)
            ?? throw new InvalidOperationException($"Produit {productId} non trouvé");

        var movement = new StockMovement
        {
            ProductId = productId,
            Type = MovementType.StockIn,
            Quantity = quantity,
            QuantityBefore = product.QuantityInStock,
            QuantityAfter = product.QuantityInStock + quantity,
            Reason = reason,
            Reference = reference,
            CreatedAt = DateTime.Now,
            CreatedBy = Environment.UserName
        };

        product.QuantityInStock += quantity;
        product.UpdatedAt = DateTime.Now;

        _context.StockMovements.Add(movement);
        await _context.SaveChangesAsync();

        return movement;
    }

    public async Task<StockMovement> RemoveStockAsync(int productId, int quantity, string? reason = null, string? reference = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("La quantité doit être positive", nameof(quantity));

        var product = await _context.Products.FindAsync(productId)
            ?? throw new InvalidOperationException($"Produit {productId} non trouvé");

        if (product.QuantityInStock < quantity)
            throw new InvalidOperationException($"Stock insuffisant. Disponible: {product.QuantityInStock}, Demandé: {quantity}");

        var movement = new StockMovement
        {
            ProductId = productId,
            Type = MovementType.StockOut,
            Quantity = quantity,
            QuantityBefore = product.QuantityInStock,
            QuantityAfter = product.QuantityInStock - quantity,
            Reason = reason,
            Reference = reference,
            CreatedAt = DateTime.Now,
            CreatedBy = Environment.UserName
        };

        product.QuantityInStock -= quantity;
        product.UpdatedAt = DateTime.Now;

        _context.StockMovements.Add(movement);
        await _context.SaveChangesAsync();

        return movement;
    }

    public async Task<StockMovement> AdjustStockAsync(int productId, int newQuantity, string? reason = null)
    {
        if (newQuantity < 0)
            throw new ArgumentException("La quantité ne peut pas être négative", nameof(newQuantity));

        var product = await _context.Products.FindAsync(productId)
            ?? throw new InvalidOperationException($"Produit {productId} non trouvé");

        var difference = newQuantity - product.QuantityInStock;

        var movement = new StockMovement
        {
            ProductId = productId,
            Type = MovementType.Adjustment,
            Quantity = Math.Abs(difference),
            QuantityBefore = product.QuantityInStock,
            QuantityAfter = newQuantity,
            Reason = reason ?? $"Ajustement de stock ({(difference >= 0 ? "+" : "")}{difference})",
            CreatedAt = DateTime.Now,
            CreatedBy = Environment.UserName
        };

        product.QuantityInStock = newQuantity;
        product.UpdatedAt = DateTime.Now;

        _context.StockMovements.Add(movement);
        await _context.SaveChangesAsync();

        return movement;
    }

    public async Task<IEnumerable<StockMovement>> GetMovementsForProductAsync(int productId)
    {
        return await _context.StockMovements
            .Where(m => m.ProductId == productId)
            .OrderByDescending(m => m.CreatedAt)
            .Take(100)
            .ToListAsync();
    }

    public async Task<IEnumerable<StockMovement>> GetRecentMovementsAsync(int count = 50)
    {
        return await _context.StockMovements
            .Include(m => m.Product)
            .OrderByDescending(m => m.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
}
