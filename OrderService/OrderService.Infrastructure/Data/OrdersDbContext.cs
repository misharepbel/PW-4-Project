using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Models;
using System.Collections.Generic;

namespace OrderService.Infrastructure.Data;
public class OrdersDbContext(DbContextOptions<OrdersDbContext> opts) : DbContext(opts)
{
    public DbSet<Order> Orders => Set<Order>();
}
