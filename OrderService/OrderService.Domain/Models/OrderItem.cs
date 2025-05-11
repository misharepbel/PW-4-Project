namespace OrderService.Domain.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Foreign key
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = default!;
    }
}