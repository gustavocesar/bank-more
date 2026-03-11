using System.Text.Json;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Options;
using Tarifas.Application.Commands.RegistrarTarifaTransferencia;
using Tarifas.Subscriber.Contracts;
using Tarifas.Subscriber.Options;

namespace Tarifas.Subscriber;

internal sealed class Worker(
    ILogger<Worker> logger,
    IOptions<KafkaOptions> kafkaOptions,
    IOptions<TarifaOptions> tarifaOptions,
    IServiceScopeFactory serviceScopeFactory)
    : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly KafkaOptions _kafkaOptions = kafkaOptions.Value;
    private readonly TarifaOptions _tarifaOptions = tarifaOptions.Value;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ValidarConfiguracao();

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            GroupId = _kafkaOptions.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            AllowAutoCreateTopics = true,
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        consumer.Subscribe(_kafkaOptions.TransferenciasTopic);

        _logger.LogInformation("Subscriber de tarifas ouvindo o topico {Topico}.", _kafkaOptions.TransferenciasTopic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var evento = JsonSerializer.Deserialize<TransferenciaRealizadaMessage>(consumeResult.Message.Value);

                if (evento is null)
                {
                    _logger.LogWarning("Mensagem invalida recebida do topico {Topico}.", consumeResult.Topic);
                    consumer.Commit(consumeResult);
                    continue;
                }

                using var scope = _serviceScopeFactory.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();

                await sender.Send(
                    new RegistrarTarifaTransferenciaCommand(
                        evento.IdContaCorrenteOrigem,
                        _tarifaOptions.ValorTarifa),
                    stoppingToken);

                consumer.Commit(consumeResult);

                _logger.LogInformation(
                    "Tarifa registrada para a conta {IdContaCorrente} a partir da transferencia {IdTransferencia}.",
                    evento.IdContaCorrenteOrigem,
                    evento.IdTransferencia);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Erro ao consumir mensagem do Kafka.");
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar a tarifacao da transferencia.");
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }

    private void ValidarConfiguracao()
    {
        if (string.IsNullOrWhiteSpace(_kafkaOptions.BootstrapServers))
            throw new InvalidOperationException("Os servidores do Kafka nao foram configurados.");

        if (_tarifaOptions.ValorTarifa <= 0)
            throw new InvalidOperationException("O valor da tarifa de transferencia deve ser maior que zero.");
    }
}
