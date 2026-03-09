using MediatR;

namespace ContaCorrente.Application.Queries.ObterSaldoContaCorrente;

public sealed record ObterSaldoContaCorrenteQuery(Guid IdContaCorrente) : IRequest<ObterSaldoContaCorrenteResponse>;

public sealed record ObterSaldoContaCorrenteResponse(
    int? NumeroConta,
    string? NomeTitular,
    DateTime? DataHoraConsulta,
    decimal? Saldo,
    string? TipoFalha,
    string? Mensagem)
{
    public bool Success => NumeroConta.HasValue;

    public static ObterSaldoContaCorrenteResponse Sucesso(
        int numeroConta,
        string nomeTitular,
        DateTime dataHoraConsulta,
        decimal saldo) =>
        new(numeroConta, nomeTitular, dataHoraConsulta, decimal.Round(saldo, 2, MidpointRounding.ToEven), null, null);

    public static ObterSaldoContaCorrenteResponse ContaInvalida(string mensagem) =>
        new(null, null, null, null, "INVALID_ACCOUNT", mensagem);

    public static ObterSaldoContaCorrenteResponse ContaInativa(string mensagem) =>
        new(null, null, null, null, "INACTIVE_ACCOUNT", mensagem);
}
