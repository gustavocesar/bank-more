using MediatR;

namespace ContaCorrente.Application.Queries.ObterContaCorrentePorId;

public sealed record ObterContaCorrentePorIdQuery(Guid IdContaCorrente) : IRequest<ObterContaCorrentePorIdResponse>;

public sealed record ObterContaCorrentePorIdResponse(Guid? IdContaCorrente, int? NumeroConta, string? Cpf, string? Nome, bool? Ativa, string? TipoFalha, string? Mensagem)
{
    public bool Success => IdContaCorrente.HasValue;

    public static ObterContaCorrentePorIdResponse Encontrada(Guid idContaCorrente, int numeroConta, string cpf, string nome, bool ativa) =>
        new(idContaCorrente, numeroConta, cpf, nome, ativa, null, null);

    public static ObterContaCorrentePorIdResponse ContaNaoEncontrada(string mensagem) =>
        new(null, null, null, null, null, "INVALID_ACCOUNT", mensagem);
}
