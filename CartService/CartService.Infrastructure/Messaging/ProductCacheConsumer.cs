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
    private IConsumer<Ignore, string>? _consumer;
    private readonly string _topic;
    private readonly IProductCache _cache;
    private readonly ILogger<ProductCacheConsumer> _logger;
    private readonly IConfiguration _configuration;

    public ProductCacheConsumer(IConfiguration configuration, IProductCache cache, ILogger<ProductCacheConsumer> logger)
    {
        _configuration = configuration;
        _cache = cache;
        _logger = logger;
        _topic = configuration["Kafka:ProductCacheTopic"] ?? "product-cache";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = "cartservice",
            BootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "kafka:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        _ = Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Attempting to connect to Kafka broker at {Broker}...", config.BootstrapServers);
                    _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
                    _consumer.Subscribe(_topic);

                    _logger.LogInformation("Kafka connection successful. Subscribed to topic '{Topic}'", _topic);

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var result = _consumer.Consume(stoppingToken);
                        var cacheEvent = JsonSerializer.Deserialize<ProductCacheEvent>(result.Message.Value);
                        if (cacheEvent != null)
                        {
                            _cache.Set(cacheEvent);
                            _logger.LogInformation("Cached product update!@!@!");
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Kafka consume exception");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kafka error. Retrying in 5 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                finally
                {
                    _consumer?.Close();
                    _consumer?.Dispose();
                }
            }
        }, stoppingToken);
    }
}
