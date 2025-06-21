namespace PaymentService.Messaging;

public interface IOrderPaidProducer
{
    Task PublishAsync(OrderPaidEvent evt, CancellationToken ct = default);
}
