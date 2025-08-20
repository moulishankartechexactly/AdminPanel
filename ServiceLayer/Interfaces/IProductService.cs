using Model.Dtos;
using System.Collections.Generic;
using System.Threading;

namespace ServiceLayer.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductDto> CreateAsync(ProductDto product, CancellationToken cancellationToken = default);
    Task UpdateAsync(ProductDto product, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountLowStockAsync(int threshold, CancellationToken cancellationToken = default);
    Task<IDictionary<string, int>> GetCountsByCategoryAsync(CancellationToken cancellationToken = default);
}
