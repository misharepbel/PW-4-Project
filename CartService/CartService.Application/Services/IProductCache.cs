using CartService.Application.DTOs;

namespace CartService.Application.Services;

public interface IProductCache
{
    IReadOnlyCollection<Guid> ProductIds { get; }
    IReadOnlyCollection<ProductDto> Products { get; }
    void Set(ProductCacheEvent cache);
    bool Contains(Guid id);
    ProductDto? Get(Guid id);
}

