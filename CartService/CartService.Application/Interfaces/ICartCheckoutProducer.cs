using CartService.Application.DTOs;

namespace CartService.Application.Interfaces;

public interface ICartCheckoutProducer
{
    Task PublishAsync(CartCheckedOutEvent evt, CancellationToken ct = default);
}
