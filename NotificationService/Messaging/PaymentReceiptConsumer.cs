using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Services;
using System.Text.Json;

namespace NotificationService.Messaging;

public class PaymentReceiptConsumer : BackgroundService
{
    private IConsumer<Ignore, string>? _consumer;
    private readonly string _topic;
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<PaymentReceiptConsumer> _logger;

    public PaymentReceiptConsumer(IConfiguration configuration, IEmailSender emailSender, ILogger<PaymentReceiptConsumer> logger)
    {
        _configuration = configuration;
        _emailSender = emailSender;
        _logger = logger;
        _topic = configuration["Kafka:PaymentReceiptTopic"] ?? "payment-receipt";
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = "notificationservice",
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
                        var message = JsonSerializer.Deserialize<EmailMessage>(result.Message.Value);
                        if (message != null)
                        {
                            await _emailSender.SendEmailAsync(message.To, message.Subject, message.Body);
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
