using MediatR;

namespace Transferencia.Application.Commands.EfetuarTransferencia;

public sealed record EfetuarTransferenciaCommand(
    Guid IdContaCorrenteOrigem,
    string IdentificacaoRequisicao,
    int NumeroContaDestino,
    decimal Valor,
    string Token
) : IRequest<EfetuarTransferenciaResponse>;

public sealed record EfetuarTransferenciaResponse(bool Success, string? TipoFalha, string? Mensagem)
{
    public static EfetuarTransferenciaResponse Sucesso() => new(true, null, null);

    public static EfetuarTransferenciaResponse ContaInvalida(string mensagem) =>
        new(false, "INVALID_ACCOUNT", mensagem);

    public static EfetuarTransferenciaResponse ContaInativa(string mensagem) =>
        new(false, "INACTIVE_ACCOUNT", mensagem);

    public static EfetuarTransferenciaResponse ValorInvalido(string mensagem) =>
        new(false, "INVALID_VALUE", mensagem);

    public static EfetuarTransferenciaResponse FalhaRequisicao(string tipoFalha, string mensagem) =>
        new(false, tipoFalha, mensagem);
}
