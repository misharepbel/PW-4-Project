namespace OrderService.Application.DTO
{
    /// <summary>
    /// DTO returned to clients representing an existing order
    /// </summary>
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
    }
}