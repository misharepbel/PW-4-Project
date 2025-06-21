using System.Collections.Generic;

namespace PaymentService.Cache;

public interface IOrderCache
{
    IReadOnlyDictionary<Guid, Guid> Orders { get; }
    bool Contains(Guid orderId);
    Guid? GetUserId(Guid orderId);
    void Add(Guid orderId, Guid userId);
}
