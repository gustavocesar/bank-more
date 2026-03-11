using MediatR;

namespace ContaCorrente.Application.Commands.DepositarContaCorrente;

public sealed record DepositarContaCorrenteCommand(
    Guid IdContaCorrenteAutenticada,
    string IdentificacaoRequisicao,
    int? NumeroConta,
    decimal Valor) : IRequest<DepositarContaCorrenteResponse>;

public sealed record DepositarContaCorrenteResponse(bool Success, string? TipoFalha, string? Mensagem)
{
    public static DepositarContaCorrenteResponse Sucesso() =>
        new(true, null, null);

    public static DepositarContaCorrenteResponse ContaInvalida(string mensagem) =>
        new(false, "INVALID_ACCOUNT", mensagem);

    public static DepositarContaCorrenteResponse ContaInativa(string mensagem) =>
        new(false, "INACTIVE_ACCOUNT", mensagem);

    public static DepositarContaCorrenteResponse ValorInvalido(string mensagem) =>
        new(false, "INVALID_VALUE", mensagem);
}
