using ContaCorrente.Domain.Repositories;
using MediatR;

namespace ContaCorrente.Application.Queries.ObterContaCorrentePorId;

internal sealed class ObterContaCorrentePorIdQueryHandler(IContaCorrenteRepository contaCorrenteRepository)
    : IRequestHandler<ObterContaCorrentePorIdQuery, ObterContaCorrentePorIdResponse>
{
    public async Task<ObterContaCorrentePorIdResponse> Handle(
        ObterContaCorrentePorIdQuery request,
        CancellationToken cancellationToken)
    {
        var contaCorrente = await contaCorrenteRepository.GetByIdAsync(request.IdContaCorrente, cancellationToken);
        if (contaCorrente is null)
            return ObterContaCorrentePorIdResponse.ContaNaoEncontrada("A conta corrente informada é inválida.");

        return ObterContaCorrentePorIdResponse.Encontrada(
            contaCorrente.Id,
            contaCorrente.NumeroConta.Value,
            contaCorrente.Cpf.Value,
            contaCorrente.Nome,
            contaCorrente.Ativo);
    }
}
