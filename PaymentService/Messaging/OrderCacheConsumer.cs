using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentService.Cache;
using System.Text.Json;

namespace PaymentService.Messaging;

public class OrderCacheConsumer : BackgroundService
{
    private IConsumer<Ignore, string>? _consumer;
    private readonly string _topic;
    private readonly IConfiguration _configuration;
    private readonly IOrderCache _cache;
    private readonly ILogger<OrderCacheConsumer> _logger;

    public OrderCacheConsumer(IConfiguration configuration, IOrderCache cache, ILogger<OrderCacheConsumer> logger)
    {
        _configuration = configuration;
        _cache = cache;
        _logger = logger;
        _topic = configuration["Kafka:OrderCacheTopic"] ?? "order-cache";
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = "paymentservice-cache",
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
                    _logger.LogInformation("Connecting to Kafka {Broker}...", config.BootstrapServers);
                    _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
                    _consumer.Subscribe(_topic);

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var result = _consumer.Consume(stoppingToken);
                        var evt = JsonSerializer.Deserialize<OrderCacheEvent>(result.Message.Value);
                        if (evt != null)
                        {
                            _cache.Set(evt);
                            _logger.LogInformation("Order cache loaded with {Count} entries", evt.Orders.Count);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kafka consumer error. Retrying in 5s...");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                finally
                {
                    _consumer?.Close();
                    _consumer?.Dispose();
                }
            }
        }, stoppingToken);

        return Task.CompletedTask;
    }
}
