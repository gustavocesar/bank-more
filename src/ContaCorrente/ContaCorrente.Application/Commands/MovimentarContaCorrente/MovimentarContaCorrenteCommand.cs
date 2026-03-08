using ContaCorrente.Domain.Enums;
using MediatR;

namespace ContaCorrente.Application.Commands.MovimentarContaCorrente;

public sealed record MovimentarContaCorrenteCommand(
    Guid IdContaCorrenteAutenticada,
    string IdentificacaoRequisicao,
    int? NumeroConta,
    decimal Valor,
    TipoMovimento TipoMovimento) : IRequest<MovimentarContaCorrenteResponse>;

public sealed record MovimentarContaCorrenteResponse(bool Success, string? TipoFalha, string? Mensagem)
{
    public static MovimentarContaCorrenteResponse Sucesso() =>
        new(true, null, null);

    public static MovimentarContaCorrenteResponse ContaInvalida(string mensagem) =>
        new(false, "INVALID_ACCOUNT", mensagem);

    public static MovimentarContaCorrenteResponse ContaInativa(string mensagem) =>
        new(false, "INACTIVE_ACCOUNT", mensagem);

    public static MovimentarContaCorrenteResponse ValorInvalido(string mensagem) =>
        new(false, "INVALID_VALUE", mensagem);

    public static MovimentarContaCorrenteResponse TipoInvalido(string mensagem) =>
        new(false, "INVALID_TYPE", mensagem);
}
