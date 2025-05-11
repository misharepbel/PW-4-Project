using OrderService.Domain.Models;

namespace OrderService.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id);
        Task AddAsync(Order order);
    }
}