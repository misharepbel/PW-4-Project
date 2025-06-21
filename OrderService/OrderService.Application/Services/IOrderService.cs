using OrderService.Application.DTO;

namespace OrderService.Application.Services
{
    public interface IOrderService
    {
        Task<Guid> CreateAsync(CreateOrderDto dto, CancellationToken ct = default);
        Task<OrderDto?> GetAsync(Guid id, CancellationToken ct = default);
    }
}
