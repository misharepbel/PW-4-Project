namespace PaymentService.Messaging;

public class OrderPaidEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
}
