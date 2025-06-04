using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using CartService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CartService.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly CartDbContext _context;
    public CartRepository(CartDbContext context) => _context = context;

    public async Task<Cart?> GetByUserIdAsync(Guid userId)
        => await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);

    public async Task AddOrUpdateItemAsync(Guid userId, CartItem item)
    {
        var cart = await GetByUserIdAsync(userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId, Items = new List<CartItem>() };
            _context.Carts.Add(cart);
        }

        var existing = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existing == null)
            cart.Items.Add(item);
        else
        {
            existing.Quantity += item.Quantity;
            existing.UnitPrice = item.UnitPrice;
            existing.ProductName = item.ProductName;
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveItemAsync(Guid userId, Guid productId)
    {
        var cart = await GetByUserIdAsync(userId);
        if (cart == null) return;
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Items.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearAsync(Guid userId)
    {
        var cart = await GetByUserIdAsync(userId);
        if (cart == null) return;
        cart.Items.Clear();
        await _context.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
