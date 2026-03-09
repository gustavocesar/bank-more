namespace ContaCorrente.API.Contracts;

public sealed record ObterSaldoContaCorrenteHttpResponse(
    int NumeroConta,
    string NomeTitular,
    DateTime DataHoraConsulta,
    decimal Saldo
);
