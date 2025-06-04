using CartService.Application.DTOs;
using CartService.Application.Services;

namespace CartService.Infrastructure.Cache;

public class ProductCache : IProductCache
{
    private List<Guid> _ids = new();

    public IReadOnlyCollection<Guid> ProductIds => _ids.AsReadOnly();

    public void Set(ProductCacheEvent cache)
    {
        _ids = cache.Products.Select(p => p.Id).ToList();
    }

    public bool Contains(Guid id) => _ids.Contains(id);
}

