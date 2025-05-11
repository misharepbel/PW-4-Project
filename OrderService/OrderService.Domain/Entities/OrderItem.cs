namespace OrderService.Domain.Entities
{
    public sealed class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}