using CatalogService.Application.DTOs;

namespace CatalogService.Application.Interfaces;

public interface IProductCacheProducer
{
    Task PublishAsync(ProductCacheEvent cache, CancellationToken ct = default);
}

