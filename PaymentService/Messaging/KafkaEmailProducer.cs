using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace PaymentService.Messaging;

public class KafkaEmailProducer : IEmailProducer, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic;

    public KafkaEmailProducer(IConfiguration configuration)
    {
        var bootstrap = configuration["Kafka:BootstrapServers"] ?? "kafka:9092";
        _topic = configuration["Kafka:PaymentReceiptTopic"] ?? "payment-receipt";
        var config = new ProducerConfig { BootstrapServers = bootstrap };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishAsync(EmailMessage message, CancellationToken ct = default)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(message);
        await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = json }, ct);
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
}
