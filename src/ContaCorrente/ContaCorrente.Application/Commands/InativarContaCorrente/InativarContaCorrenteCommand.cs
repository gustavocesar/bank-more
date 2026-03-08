using MediatR;

namespace ContaCorrente.Application.Commands.InativarContaCorrente;

public sealed record InativarContaCorrenteCommand(Guid IdContaCorrente, string Senha) : IRequest<InativarContaCorrenteResponse>;

public sealed record InativarContaCorrenteResponse(bool Success, string? TipoFalha, string? Mensagem)
{
    public static InativarContaCorrenteResponse ContaInativada() =>
        new(true, null, null);

    public static InativarContaCorrenteResponse ContaInvalida(string mensagem) =>
        new(false, "INVALID_ACCOUNT", mensagem);

    public static InativarContaCorrenteResponse SenhaInvalida(string mensagem) =>
        new(false, "INVALID_PASSWORD", mensagem);
}
