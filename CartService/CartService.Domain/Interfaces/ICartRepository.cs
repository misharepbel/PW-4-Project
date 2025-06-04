using CartService.Domain.Entities;

namespace CartService.Domain.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(Guid userId);
    Task<List<Cart>> GetAllAsync();
    Task AddOrUpdateItemAsync(Guid userId, CartItem item);
    Task RemoveItemAsync(Guid userId, Guid productId);
    Task ClearAsync(Guid userId);
    Task SaveChangesAsync();
}
