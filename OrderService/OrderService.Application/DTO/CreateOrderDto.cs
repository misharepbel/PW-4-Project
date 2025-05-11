namespace OrderService.Application.DTO
{
    /// <summary>
    /// DTO used when creating a new order from client input
    /// </summary>
    public class CreateOrderDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}