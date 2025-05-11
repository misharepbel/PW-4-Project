using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;
using OrderService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersDbContext _db;

        public OrderRepository(OrdersDbContext db) => _db = db;

        public async Task AddAsync(Order order)
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
        }

        public Task<Order?> GetByIdAsync(Guid id) =>
            _db.Orders
               .Include(o => o.OrderItems)
               .SingleOrDefaultAsync(o => o.Id == id);

        public async Task<List<Order>> GetAllAsync() =>
            await _db.Orders
                     .Include(o => o.OrderItems)
                     .ToListAsync();

        public async Task DeleteAsync(Guid id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order is null) return;

            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
        }

        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
}
