using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
    public OrderStatus Status { get; set; } = OrderStatus.New;
    public string DeliveryLocation { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
}
