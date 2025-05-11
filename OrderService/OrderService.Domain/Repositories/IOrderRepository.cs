using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderService.Domain.Entities;

namespace OrderService.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order?> GetByIdAsync(Guid id);
        Task<List<Order>> GetAllAsync();
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
