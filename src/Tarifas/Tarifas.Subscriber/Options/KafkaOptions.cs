namespace Tarifas.Subscriber.Options;

internal sealed class KafkaOptions
{
    public const string SectionName = "Kafka";

    public string BootstrapServers { get; init; } = string.Empty;

    public string TransferenciasTopic { get; init; } = "TransferenciaRealizadaEvent";

    public string GroupId { get; init; } = "tarifas-subscriber";
}
