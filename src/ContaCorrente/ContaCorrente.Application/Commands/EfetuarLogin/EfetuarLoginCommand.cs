using MediatR;

namespace ContaCorrente.Application.Commands.EfetuarLogin;

public sealed record EfetuarLoginCommand(string NumeroContaOuCpf, string Senha) : IRequest<EfetuarLoginResponse>;

public sealed record EfetuarLoginResponse(Guid? IdContaCorrente, string? TipoFalha, string? Mensagem)
{
    public bool Success => IdContaCorrente.HasValue;

    public static EfetuarLoginResponse Autenticado(Guid idContaCorrente) =>
        new(idContaCorrente, null, null);

    public static EfetuarLoginResponse NaoAutorizado(string mensagem) =>
        new(null, "USER_UNAUTHORIZED", mensagem);
}