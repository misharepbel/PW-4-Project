namespace OrderService.Application.DTO
{
    /// <summary>
    /// DTO used when creating a new order item from client input
    /// </summary>
    public class CreateOrderItemDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}