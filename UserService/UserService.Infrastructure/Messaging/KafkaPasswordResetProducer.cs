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

    public async Task PublishAsync(PasswordResetEvent evt, CancellationToken ct = default)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(evt);
        await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = json }, ct);
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
}
