namespace PaymentService.Messaging;

public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
}
