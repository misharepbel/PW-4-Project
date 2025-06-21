using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PaymentService.Cache;

public class OrderCache : IOrderCache
{
    private readonly ConcurrentDictionary<Guid, Guid> _orders = new();

    public IReadOnlyDictionary<Guid, Guid> Orders => _orders;

    public void Set(Messaging.OrderCacheEvent cache)
    {
        _orders.Clear();
        foreach (var kvp in cache.Orders)
        {
            _orders[kvp.Key] = kvp.Value;
        }
    }

    public bool Contains(Guid orderId) => _orders.ContainsKey(orderId);

    public Guid? GetUserId(Guid orderId) => _orders.TryGetValue(orderId, out var userId) ? userId : null;

    public void Add(Guid orderId, Guid userId)
    {
        _orders[orderId] = userId;
    }
}
