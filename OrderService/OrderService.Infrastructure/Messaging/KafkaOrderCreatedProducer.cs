using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using OrderService.Application.DTO;
using OrderService.Application.Interfaces;

namespace OrderService.Infrastructure.Messaging;

public class KafkaOrderCreatedProducer : IOrderCreatedProducer, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic;

    public KafkaOrderCreatedProducer(IConfiguration configuration)
    {
        var bootstrap = configuration["Kafka:BootstrapServers"] ?? "kafka:9092";
        _topic = configuration["Kafka:OrderCreatedTopic"] ?? "order-created";
        var config = new ProducerConfig { BootstrapServers = bootstrap };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishAsync(OrderCreatedEvent evt, CancellationToken ct = default)
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
