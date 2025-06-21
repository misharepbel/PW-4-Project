using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using OrderService.Application.Commands;
using OrderService.Application.DTO;

namespace OrderService.Infrastructure.Messaging;

public class CartCheckoutConsumer : BackgroundService
{
    private IConsumer<Ignore, string>? _consumer;
    private readonly string _topic;
    private readonly IMediator _mediator;
    private readonly ILogger<CartCheckoutConsumer> _logger;
    private readonly IConfiguration _configuration;

    public CartCheckoutConsumer(IConfiguration configuration, IMediator mediator, ILogger<CartCheckoutConsumer> logger)
    {
        _configuration = configuration;
        _mediator = mediator;
        _logger = logger;
        _topic = configuration["Kafka:CartCheckedOutTopic"] ?? "cart-checked-out";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
                        var evt = System.Text.Json.JsonSerializer.Deserialize<CartCheckedOutEvent>(result.Message.Value);
                        if (evt != null)
                        {
                            var dto = new CreateOrderDto
                            {
                                UserId = evt.UserId,
                                DeliveryLocation = evt.DeliveryLocation,
                                PaymentMethod = evt.PaymentMethod,
                                Items = evt.Items.Select(i => new CreateOrderItemDto
                                {
                                    ProductId = i.ProductId,
                                    ProductName = i.ProductName,
                                    Quantity = i.Quantity,
                                    UnitPrice = i.UnitPrice,
                                    TotalPrice = i.TotalPrice
                                }).ToList()
                            };
                            await _mediator.Send(new CreateOrderCommand(dto), stoppingToken);
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
    }
}
