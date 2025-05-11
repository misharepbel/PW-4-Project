namespace OrderService.Domain.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = default!;
        public List<OrderItem> Items { get; set; } = new();
    }
}