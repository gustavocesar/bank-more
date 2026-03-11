using ContaCorrente.Domain.Enums;
using ContaCorrente.Domain.Repositories;
using MediatR;
using MovimentoEntity = ContaCorrente.Domain.Entities.Movimento;

namespace ContaCorrente.Application.Commands.DepositarContaCorrente;

internal sealed class DepositarContaCorrenteCommandHandler(
    IContaCorrenteRepository contaCorrenteRepository,
    IMovimentoRepository movimentoRepository)
    : IRequestHandler<DepositarContaCorrenteCommand, DepositarContaCorrenteResponse>
{
    public async Task<DepositarContaCorrenteResponse> Handle(
        DepositarContaCorrenteCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Valor <= 0)
            return DepositarContaCorrenteResponse.ValorInvalido("Apenas valores positivos podem ser depositados.");

        var contaCorrente = request.NumeroConta.HasValue
            ? await contaCorrenteRepository.GetByNumeroOrCpfAsync(request.NumeroConta.Value.ToString(), cancellationToken)
            : await contaCorrenteRepository.GetByIdAsync(request.IdContaCorrenteAutenticada, cancellationToken);

        if (contaCorrente is null)
            return DepositarContaCorrenteResponse.ContaInvalida("A conta corrente informada é inválida.");

        if (!contaCorrente.Ativo)
            return DepositarContaCorrenteResponse.ContaInativa("A conta corrente informada está inativa.");

        var movimento = MovimentoEntity.Criar(contaCorrente.Id, TipoMovimento.C, request.Valor);
        await movimentoRepository.CreateAsync(movimento, cancellationToken);

        return DepositarContaCorrenteResponse.Sucesso();
    }
}
