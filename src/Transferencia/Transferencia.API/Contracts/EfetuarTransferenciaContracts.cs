namespace Transferencia.API.Contracts;

public sealed class EfetuarTransferenciaRequest
{
    public required int NumeroContaDestino { get; init; }
    public required decimal Valor { get; init; }
}
