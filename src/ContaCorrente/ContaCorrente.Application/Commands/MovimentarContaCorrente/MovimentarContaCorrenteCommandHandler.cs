using ContaCorrente.Domain.Enums;
using ContaCorrente.Domain.Repositories;
using MediatR;
using MovimentoEntity = ContaCorrente.Domain.Entities.Movimento;

namespace ContaCorrente.Application.Commands.MovimentarContaCorrente;

internal sealed class MovimentarContaCorrenteCommandHandler(
    IContaCorrenteRepository contaCorrenteRepository,
    IMovimentoRepository movimentoRepository)
    : IRequestHandler<MovimentarContaCorrenteCommand, MovimentarContaCorrenteResponse>
{
    public async Task<MovimentarContaCorrenteResponse> Handle(
        MovimentarContaCorrenteCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Valor <= 0)
            return MovimentarContaCorrenteResponse.ValorInvalido("Apenas valores positivos podem ser movimentados.");

        if (request.TipoMovimento is not (TipoMovimento.C or TipoMovimento.D))
            return MovimentarContaCorrenteResponse.TipoInvalido("Apenas os tipos de movimento 'C' ou 'D' podem ser aceitos.");

        var contaCorrente = request.NumeroConta.HasValue
            ? await contaCorrenteRepository.GetByNumeroOrCpfAsync(request.NumeroConta.Value.ToString(), cancellationToken)
            : await contaCorrenteRepository.GetByIdAsync(request.IdContaCorrenteAutenticada, cancellationToken);

        if (contaCorrente is null)
            return MovimentarContaCorrenteResponse.ContaInvalida("A conta corrente informada é inválida.");

        if (!contaCorrente.Ativo)
            return MovimentarContaCorrenteResponse.ContaInativa("A conta corrente informada está inativa.");

        if (contaCorrente.Id != request.IdContaCorrenteAutenticada && request.TipoMovimento != TipoMovimento.C)
        {
            return MovimentarContaCorrenteResponse.TipoInvalido(
                "Apenas o tipo 'C' pode ser aceito quando a conta informada for diferente do usuário autenticado.");
        }

        var movimento = MovimentoEntity.Criar(contaCorrente.Id, request.TipoMovimento, request.Valor);
        await movimentoRepository.CreateAsync(movimento, cancellationToken);

        return MovimentarContaCorrenteResponse.Sucesso();
    }
}
