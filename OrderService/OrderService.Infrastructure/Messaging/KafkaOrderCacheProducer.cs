using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using OrderService.Application.DTO;
using OrderService.Application.Interfaces;
using System.Text.Json;

namespace OrderService.Infrastructure.Messaging;

public class KafkaOrderCacheProducer : IOrderCacheProducer, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic;

    public KafkaOrderCacheProducer(IConfiguration configuration)
    {
        var bootstrap = configuration["Kafka:BootstrapServers"] ?? "kafka:9092";
        _topic = configuration["Kafka:OrderCacheTopic"] ?? "order-cache";
        var config = new ProducerConfig { BootstrapServers = bootstrap };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishAsync(OrderCacheEvent cache, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(cache);
        await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = json }, ct);
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
}
