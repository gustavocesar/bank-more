using ContaCorrente.Domain.Repositories;
using MediatR;

namespace ContaCorrente.Application.Queries.ObterContaCorrentePorNumero;

internal sealed class ObterContaCorrentePorNumeroQueryHandler(IContaCorrenteRepository contaCorrenteRepository)
    : IRequestHandler<ObterContaCorrentePorNumeroQuery, ObterContaCorrentePorNumeroResponse>
{
    public async Task<ObterContaCorrentePorNumeroResponse> Handle(
        ObterContaCorrentePorNumeroQuery request,
        CancellationToken cancellationToken)
    {
        if (request.NumeroConta is 0 or <= 100000)
            return ObterContaCorrentePorNumeroResponse.NumeroContaInvalido("O número da conta corrente informado é inválido.");

        var contaCorrente = await contaCorrenteRepository.GetByNumeroOrCpfAsync(request.NumeroConta.ToString(), cancellationToken);
        if (contaCorrente is null)
            return ObterContaCorrentePorNumeroResponse.ContaNaoEncontrada("A conta corrente informada é inválida.");

        return ObterContaCorrentePorNumeroResponse.Encontrada(
            contaCorrente.Id,
            contaCorrente.NumeroConta.Value,
            contaCorrente.Cpf.Value,
            contaCorrente.Nome,
            contaCorrente.Ativo);
    }
}
