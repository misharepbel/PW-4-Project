namespace PaymentService.Messaging;

public interface IEmailProducer
{
    Task PublishAsync(EmailMessage message, CancellationToken ct = default);
}
