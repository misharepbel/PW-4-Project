using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Models;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository(OrdersDbContext db) : IOrderRepository
    {
        private readonly OrdersDbContext _db = db;

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _db.Orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task AddAsync(Order order)
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
        }
    }
}
