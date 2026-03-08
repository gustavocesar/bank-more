using MediatR;

namespace ContaCorrente.Application.Queries.ObterContaCorrentePorNumero;

public sealed record ObterContaCorrentePorNumeroQuery(int NumeroConta) : IRequest<ObterContaCorrentePorNumeroResponse>;

public sealed record ObterContaCorrentePorNumeroResponse(Guid? IdContaCorrente, int? NumeroConta, string? Cpf, string? Nome, bool? Ativa, string? TipoFalha, string? Mensagem)
{
    public bool Success => IdContaCorrente.HasValue;

    public static ObterContaCorrentePorNumeroResponse Encontrada(Guid idContaCorrente, int numeroConta, string cpf, string nome, bool ativa) =>
        new(idContaCorrente, numeroConta, cpf, nome, ativa, null, null);

    public static ObterContaCorrentePorNumeroResponse NumeroContaInvalido(string mensagem) =>
        new(null, null, null, null, null, "INVALID_ACCOUNT_NUMBER", mensagem);

    public static ObterContaCorrentePorNumeroResponse ContaNaoEncontrada(string mensagem) =>
        new(null, null, null, null, null, "INVALID_ACCOUNT", mensagem);
}
