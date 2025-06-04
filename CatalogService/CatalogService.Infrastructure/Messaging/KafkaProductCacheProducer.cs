using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace CatalogService.Infrastructure.Messaging;

public class KafkaProductCacheProducer : IProductCacheProducer, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic;

    public KafkaProductCacheProducer(IConfiguration configuration)
    {
        var bootstrap = configuration["Kafka:BootstrapServers"] ?? "kafka:9092";
        _topic = configuration["Kafka:ProductCacheTopic"] ?? "product-cache";
        var config = new ProducerConfig { BootstrapServers = bootstrap };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishAsync(ProductCacheEvent cache, CancellationToken ct = default)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(cache);
        await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = json }, ct);
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
}

