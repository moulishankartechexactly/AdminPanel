using DatabaseLayer.Interfaces;
using Entity.Models;
using Model.Dtos;
using ServiceLayer.Interfaces;
using System.Collections.Generic;

namespace ServiceLayer.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _products;
    
    public ProductService(IProductRepository products)
    {
        _products = products;
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _products.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _products.GetAllAsync(cancellationToken);
        return list.Select(ToDto).ToList();
    }

    public async Task<ProductDto> CreateAsync(ProductDto product, CancellationToken cancellationToken = default)
    {
        var entity = FromDto(product);
        await _products.AddAsync(entity, cancellationToken);
        await _products.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    public async Task UpdateAsync(ProductDto product, CancellationToken cancellationToken = default)
    {
        var existing = await _products.GetByIdAsync(product.Id, cancellationToken);
        if (existing is null) return;
        existing.Name = product.Name;
        existing.Category = product.Category;
        existing.Price = product.Price;
        existing.StockQuantity = product.StockQuantity;
        existing.Description = product.Description;
        existing.ImageUrl = product.ImageUrl;
        _products.Update(existing);
        await _products.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _products.GetByIdAsync(id, cancellationToken);
        if (entity is null) return;
        _products.Remove(entity);
        await _products.SaveChangesAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _products.CountAsync(cancellationToken);

    public Task<int> CountLowStockAsync(int threshold, CancellationToken cancellationToken = default)
        => _products.CountLowStockAsync(threshold, cancellationToken);

    public Task<IDictionary<string, int>> GetCountsByCategoryAsync(CancellationToken cancellationToken = default)
        => _products.GetCountsByCategoryAsync(cancellationToken);

    private static ProductDto ToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Category = p.Category,
        Price = p.Price,
        StockQuantity = p.StockQuantity,
        Description = p.Description,
        ImageUrl = p.ImageUrl
    };

    private static Product FromDto(ProductDto d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        Category = d.Category,
        Price = d.Price,
        StockQuantity = d.StockQuantity,
        Description = d.Description,
        ImageUrl = d.ImageUrl
    };
}
