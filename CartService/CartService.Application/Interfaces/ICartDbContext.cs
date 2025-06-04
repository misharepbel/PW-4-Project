using CartService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartService.Application.Interfaces;

public interface ICartDbContext
{
    DbSet<Cart> Carts { get; }
    DbSet<CartItem> CartItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
