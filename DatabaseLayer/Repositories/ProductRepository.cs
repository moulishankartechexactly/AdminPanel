using DatabaseLayer.Data;
using DatabaseLayer.Interfaces;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseLayer.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _context.Set<Product>().CountAsync(cancellationToken);

    public async Task<int> CountLowStockAsync(int threshold, CancellationToken cancellationToken = default)
        => await _context.Set<Product>().CountAsync(p => p.StockQuantity < threshold, cancellationToken);

    public async Task<IDictionary<string, int>> GetCountsByCategoryAsync(CancellationToken cancellationToken = default)
        => await _context.Set<Product>()
            .AsNoTracking()
            .GroupBy(p => p.Category ?? "Uncategorized")
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Category, x => x.Count, cancellationToken);
}
