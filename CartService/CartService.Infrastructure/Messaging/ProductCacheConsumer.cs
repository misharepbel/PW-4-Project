using CartService.Application.DTOs;
using CartService.Application.Services;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CartService.Infrastructure.Messaging;

public class ProductCacheConsumer : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly string _topic;
    private readonly IProductCache _cache;
    private readonly ILogger<ProductCacheConsumer> _logger;

    public ProductCacheConsumer(IConfiguration configuration, IProductCache cache, ILogger<ProductCacheConsumer> logger)
    {
        var config = new ConsumerConfig
        {
            GroupId = "cartservice",
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _topic = configuration["Kafka:ProductCacheTopic"] ?? "product-cache";
        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _cache = cache;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(stoppingToken);
                var cacheEvent = JsonSerializer.Deserialize<ProductCacheEvent>(result.Message.Value);
                if (cacheEvent != null)
                    _cache.Set(cacheEvent);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming product cache");
            }
        }
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}

