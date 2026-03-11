namespace ContaCorrente.API.Contracts;

public sealed class DepositarContaCorrenteRequest
{
    public int? NumeroConta { get; init; }
    public decimal Valor { get; init; }
}
