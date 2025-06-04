using AutoMapper;
using CartService.Application.DTOs;
using CartService.Domain.Entities;
using CartService.Domain.Interfaces;

namespace CartService.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _repo;
    private readonly IMapper _mapper;

    public CartService(ICartRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<CartDto?> GetAsync(Guid userId, CancellationToken ct = default)
    {
        var cart = await _repo.GetByUserIdAsync(userId);
        return cart is null ? null : _mapper.Map<CartDto>(cart);
    }

    public async Task AddItemAsync(Guid userId, CartItemDto itemDto, CancellationToken ct = default)
    {
        var item = _mapper.Map<CartItem>(itemDto);
        item.Id = Guid.NewGuid();
        item.CartUserId = userId;
        await _repo.AddOrUpdateItemAsync(userId, item);
    }

    public Task RemoveItemAsync(Guid userId, Guid productId, CancellationToken ct = default)
        => _repo.RemoveItemAsync(userId, productId);

    public Task ClearAsync(Guid userId, CancellationToken ct = default)
        => _repo.ClearAsync(userId);
}
