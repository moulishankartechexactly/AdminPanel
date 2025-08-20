using Entity.Models;
using System.Collections.Generic;
using System.Threading;

namespace DatabaseLayer.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountLowStockAsync(int threshold, CancellationToken cancellationToken = default);
    Task<IDictionary<string, int>> GetCountsByCategoryAsync(CancellationToken cancellationToken = default);
}
