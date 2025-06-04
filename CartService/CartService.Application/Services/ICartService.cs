using CartService.Application.DTOs;

namespace CartService.Application.Services;

public interface ICartService
{
    Task<CartDto?> GetAsync(Guid userId, CancellationToken ct = default);
    Task<List<CartDto>> GetAllAsync(CancellationToken ct = default);
    Task AddItemAsync(Guid userId, CartItemDto item, CancellationToken ct = default);
    Task RemoveItemAsync(Guid userId, Guid productId, CancellationToken ct = default);
    Task ClearAsync(Guid userId, CancellationToken ct = default);
}
