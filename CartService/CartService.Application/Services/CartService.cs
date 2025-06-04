using AutoMapper;
using CartService.Application.DTOs;
using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using CartService.Application.Services;
using System.Linq;

namespace CartService.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _repo;
    private readonly IMapper _mapper;
    private readonly IProductCache _cache;
    public CartService(ICartRepository repo, IMapper mapper, IProductCache cache)
    {
        _repo = repo;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<CartDto?> GetAsync(Guid userId, CancellationToken ct = default)
    {
        var cart = await _repo.GetByUserIdAsync(userId);
        return cart is null ? null : _mapper.Map<CartDto>(cart);
    }

    public async Task<List<CartDto>> GetAllAsync(CancellationToken ct = default)
    {
        var carts = await _repo.GetAllAsync();
        return carts.Select(c => _mapper.Map<CartDto>(c)).ToList();
    }

    public async Task AddItemAsync(Guid userId, CartItemDto itemDto, CancellationToken ct = default)
    {
        if (!_cache.Contains(itemDto.ProductId))
            throw new ArgumentException("Invalid product id");

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
