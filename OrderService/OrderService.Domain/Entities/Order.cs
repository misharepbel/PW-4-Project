using OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }         
    public DateTime OrderDate { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
    public string Status { get; set; } = "New";
}
