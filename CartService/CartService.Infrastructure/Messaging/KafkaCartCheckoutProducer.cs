using CartService.Application.DTOs;
using CartService.Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace CartService.Infrastructure.Messaging;

public class KafkaCartCheckoutProducer : ICartCheckoutProducer, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic;

    public KafkaCartCheckoutProducer(IConfiguration configuration)
    {
        var bootstrap = configuration["Kafka:BootstrapServers"] ?? "kafka:9092";
        _topic = configuration["Kafka:CartCheckedOutTopic"] ?? "cart-checked-out";
        var config = new ProducerConfig { BootstrapServers = bootstrap };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishAsync(CartCheckedOutEvent evt, CancellationToken ct = default)
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
