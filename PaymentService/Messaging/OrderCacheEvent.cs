namespace PaymentService.Messaging;

public class OrderCacheEvent
{
    public Dictionary<Guid, Guid> Orders { get; set; } = new();
}
