namespace OrderService.Application.DTO;

public class CartCheckedOutEvent
{
    public Guid UserId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}
