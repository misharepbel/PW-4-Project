using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure.Messaging;

public class KafkaUserRegisteredProducer : IUserRegisteredProducer, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic;

    public KafkaUserRegisteredProducer(IConfiguration configuration)
    {
        var bootstrap = configuration["Kafka:BootstrapServers"] ?? "kafka:9092";
        _topic = configuration["Kafka:UserRegisteredTopic"] ?? "user-registered";
        var config = new ProducerConfig { BootstrapServers = bootstrap };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    private record EmailMessage(string To, string Subject, string Body);

    public async Task PublishAsync(UserRegisteredEvent evt, CancellationToken ct = default)
    {
        var message = new EmailMessage(
            evt.Email,
            "Welcome to TeaShop",
            $"Hello {evt.Username}, thank you for registering!");

        var json = System.Text.Json.JsonSerializer.Serialize(message);
        await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = json }, ct);
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
}
