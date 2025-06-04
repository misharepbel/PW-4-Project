using CartService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using CartService.Application.Interfaces;

namespace CartService.Infrastructure.Persistence;

public class CartDbContext : DbContext, ICartDbContext
{
    public CartDbContext(DbContextOptions<CartDbContext> options) : base(options) { }

    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
}
