using OrderService.Application.DTO;

namespace OrderService.Application.Interfaces;

public interface IOrderCacheProducer
{
    Task PublishAsync(OrderCacheEvent cache, CancellationToken ct = default);
}
