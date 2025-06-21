namespace OrderService.Application.DTO;

public class OrderCacheEvent
{
    public Dictionary<Guid, Guid> Orders { get; set; } = new();
}
