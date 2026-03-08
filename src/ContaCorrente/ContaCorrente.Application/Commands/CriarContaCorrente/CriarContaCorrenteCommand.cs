using MediatR;

namespace ContaCorrente.Application.Commands.CriarContaCorrente;

public sealed record CriarContaCorrenteCommand(string Cpf, string Senha) : IRequest<CriarContaCorrenteResponse>;

public sealed record CriarContaCorrenteResponse(int? NumeroConta, string? TipoFalha, string? Mensagem)
{
    public bool Success => NumeroConta.HasValue;

    public static CriarContaCorrenteResponse Criada(int numeroConta) =>
        new(numeroConta, null, null);

    public static CriarContaCorrenteResponse DocumentoInvalido(string mensagem) =>
        new(null, "INVALID_DOCUMENT", mensagem);

    public static CriarContaCorrenteResponse ContaAtivaJaExistente(string mensagem) =>
        new(null, "ACTIVE_ACCOUNT_ALREADY_EXISTS", mensagem);
}