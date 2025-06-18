using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure.Messaging;

public class KafkaPasswordResetProducer : IPasswordResetProducer, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic;

    public KafkaPasswordResetProducer(IConfiguration configuration)
    {
        var bootstrap = configuration["Kafka:BootstrapServers"] ?? "kafka:9092";
        _topic = configuration["Kafka:PasswordResetTopic"] ?? "password-reset";
        var config = new ProducerConfig { BootstrapServers = bootstrap };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    private record EmailMessage(string To, string Subject, string Body);

    public async Task PublishAsync(PasswordResetEvent evt, CancellationToken ct = default)
    {
        var resetLink = $"https://example.com/reset?token={evt.ResetToken}";
        var message = new EmailMessage(
            evt.Email,
            "Password Reset Request",
            $"Click <a href='{resetLink}'>here</a> to reset your password.");

        var json = System.Text.Json.JsonSerializer.Serialize(message);
        await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = json }, ct);
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
}
