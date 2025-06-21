using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTO;
using OrderService.Domain.Enums;
using OrderService.Domain.Repositories;
using System.Text.Json;

namespace OrderService.Infrastructure.Messaging;

public class OrderPaidConsumer : BackgroundService
{
    private IConsumer<Ignore, string>? _consumer;
    private readonly string _topic;
    private readonly IConfiguration _configuration;
    private readonly IOrderRepository _repository;
    private readonly ILogger<OrderPaidConsumer> _logger;

    public OrderPaidConsumer(IConfiguration configuration, IOrderRepository repository, ILogger<OrderPaidConsumer> logger)
    {
        _configuration = configuration;
        _repository = repository;
        _logger = logger;
        _topic = configuration["Kafka:OrderPaidTopic"] ?? "order-paid";
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = "orderservice",
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
                    _logger.LogInformation("Subscribed to topic '{Topic}'", _topic);

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var result = _consumer.Consume(stoppingToken);
                        var evt = JsonSerializer.Deserialize<OrderPaidEvent>(result.Message.Value);
                        if (evt != null)
                        {
                            var order = await _repository.GetByIdAsync(evt.OrderId);
                            if (order != null)
                            {
                                order.Status = OrderStatus.Paid;
                                await _repository.SaveChangesAsync();
                            }
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
