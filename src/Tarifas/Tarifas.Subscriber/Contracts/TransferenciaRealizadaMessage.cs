namespace Tarifas.Subscriber.Contracts;

internal sealed record TransferenciaRealizadaMessage(
    Guid IdTransferencia,
    Guid IdContaCorrenteOrigem,
    Guid IdContaCorrenteDestino,
    decimal Valor,
    DateOnly DataMovimento
);
