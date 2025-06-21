using CartService.Application.DTOs;
using CartService.Application.Services;

namespace CartService.Infrastructure.Cache;

public class ProductCache : IProductCache
{
    private Dictionary<Guid, ProductDto> _products = new();

    public IReadOnlyCollection<Guid> ProductIds => _products.Keys.ToList();

    public IReadOnlyCollection<ProductDto> Products => _products.Values.ToList();

    public void Set(ProductCacheEvent cache)
    {
        _products = cache.Products.ToDictionary(p => p.Id);
    }

    public bool Contains(Guid id) => _products.ContainsKey(id);

    public ProductDto? Get(Guid id) => _products.TryGetValue(id, out var p) ? p : null;
}

