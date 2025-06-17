namespace CartService.Application.DTOs;

public class CartCheckedOutEvent
{
    public Guid UserId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
}
