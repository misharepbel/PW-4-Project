namespace OrderService.Application.DTO
{
    public sealed class CreateOrderItemDto
    {
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = "Unknown";
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal TotalPrice { get; init; }
    }
}