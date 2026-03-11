using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using SharedKernel.Messaging;

namespace Transferencia.Infrastructure.Messaging;

internal sealed class KafkaEventPublisher(IConfiguration configuration) : IEventPublisher, IDisposable
{
    private readonly IProducer<string, string> producer = new ProducerBuilder<string, string>(
        new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"]
                ?? throw new InvalidOperationException("Os servidores do Kafka nao foram configurados."),
        }).Build();

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : class
    {
        var topic = typeof(TEvent).Name;
        var message = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(@event),
        };

        await producer.ProduceAsync(topic, message, cancellationToken);
    }

    public void Dispose() => producer.Dispose();
}
