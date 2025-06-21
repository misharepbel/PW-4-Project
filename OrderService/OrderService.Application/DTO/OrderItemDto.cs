public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = "Unknown";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
}
