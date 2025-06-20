namespace PaymentService.Messaging;

public class OrderPaidEvent
{
    public Guid OrderId { get; set; }
}
