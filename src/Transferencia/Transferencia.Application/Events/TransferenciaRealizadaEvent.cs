namespace Transferencia.Application.Events;

public sealed record TransferenciaRealizadaEvent(
    Guid IdTransferencia,
    Guid IdContaCorrenteOrigem,
    Guid IdContaCorrenteDestino,
    decimal Valor,
    DateOnly DataMovimento
);
