namespace OrderService.Application.Interfaces;

public interface IEmailProducer
{
    Task PublishAsync(EmailMessage message, CancellationToken ct = default);
}
