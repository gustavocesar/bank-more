using ContaCorrente.Domain.Repositories;
using MediatR;

namespace ContaCorrente.Application.Queries.ObterSaldoContaCorrente;

internal sealed class ObterSaldoContaCorrenteQueryHandler(
    IContaCorrenteRepository contaCorrenteRepository,
    IMovimentoRepository movimentoRepository)
    : IRequestHandler<ObterSaldoContaCorrenteQuery, ObterSaldoContaCorrenteResponse>
{
    public async Task<ObterSaldoContaCorrenteResponse> Handle(
        ObterSaldoContaCorrenteQuery request,
        CancellationToken cancellationToken)
    {
        var contaCorrente = await contaCorrenteRepository.GetByIdAsync(request.IdContaCorrente, cancellationToken);
        if (contaCorrente is null)
            return ObterSaldoContaCorrenteResponse.ContaInvalida("Apenas contas correntes cadastradas podem consultar o saldo.");

        if (!contaCorrente.Ativo)
            return ObterSaldoContaCorrenteResponse.ContaInativa("Apenas contas correntes ativas podem consultar o saldo.");

        var saldo = await movimentoRepository.GetSaldoAsync(contaCorrente.Id, cancellationToken);

        return ObterSaldoContaCorrenteResponse.Sucesso(
            contaCorrente.NumeroConta.Value,
            contaCorrente.Nome,
            DateTime.UtcNow,
            saldo
        );
    }
}
