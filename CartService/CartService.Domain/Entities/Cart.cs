namespace CartService.Domain.Entities;

public class Cart
{
    public Guid UserId { get; set; }
    public List<CartItem> Items { get; set; } = new();
}
