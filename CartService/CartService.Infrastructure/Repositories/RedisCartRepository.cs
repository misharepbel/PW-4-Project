using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace CartService.Infrastructure.Repositories;

public class RedisCartRepository : ICartRepository
{
    private readonly IDatabase _db;
    private readonly IConnectionMultiplexer _connection;

    public RedisCartRepository(IConnectionMultiplexer connection)
    {
        _connection = connection;
        _db = connection.GetDatabase();
    }

    private static string GetKey(Guid userId) => $"cart:{userId}";

    public async Task<Cart?> GetByUserIdAsync(Guid userId)
    {
        var value = await _db.StringGetAsync(GetKey(userId));
        if (value.IsNullOrEmpty) return null;
        return JsonSerializer.Deserialize<Cart>(value!);
    }

    public async Task AddOrUpdateItemAsync(Guid userId, CartItem item)
    {
        var cart = await GetByUserIdAsync(userId) ?? new Cart { UserId = userId, Items = new List<CartItem>() };
        var existing = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existing == null)
            cart.Items.Add(item);
        else
        {
            existing.Quantity += item.Quantity;
            existing.UnitPrice = item.UnitPrice;
            existing.ProductName = item.ProductName;
        }
        await _db.StringSetAsync(GetKey(userId), JsonSerializer.Serialize(cart));
    }

    public async Task RemoveItemAsync(Guid userId, Guid productId)
    {
        var cart = await GetByUserIdAsync(userId);
        if (cart == null) return;
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Items.Remove(item);
            await _db.StringSetAsync(GetKey(userId), JsonSerializer.Serialize(cart));
        }
    }

    public async Task ClearAsync(Guid userId)
    {
        await _db.KeyDeleteAsync(GetKey(userId));
    }

    public async Task<List<Cart>> GetAllAsync()
    {
        var server = _connection.GetServer(_connection.GetEndPoints().First());
        var keys = server.Keys(pattern: "cart:*");
        var carts = new List<Cart>();
        foreach (var key in keys)
        {
            var value = await _db.StringGetAsync(key);
            if (!value.IsNullOrEmpty)
            {
                var cart = JsonSerializer.Deserialize<Cart>(value!);
                if (cart != null)
                    carts.Add(cart);
            }
        }
        return carts;
    }

    public Task SaveChangesAsync() => Task.CompletedTask;
}
