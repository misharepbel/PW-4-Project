namespace OrderService.Application.DTO
{
    /// <summary>
    /// DTO returned to clients representing an existing order item
    /// </summary>
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}