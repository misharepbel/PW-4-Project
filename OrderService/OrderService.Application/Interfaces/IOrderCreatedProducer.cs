using OrderService.Application.DTO;

namespace OrderService.Application.Interfaces;

public interface IOrderCreatedProducer
{
    Task PublishAsync(OrderCreatedEvent evt, CancellationToken ct = default);
}
