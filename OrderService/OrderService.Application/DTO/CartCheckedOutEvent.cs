namespace OrderService.Application.DTO;

public class CartCheckedOutEvent
{
    public Guid UserId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
    public string DeliveryLocation { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
}
