using MediatR;
using Transferencia.Domain.Repositories;
using Transferencia.Domain.Services;
using TransferenciaEntity = Transferencia.Domain.Entities.Transferencia;

namespace Transferencia.Application.Commands.EfetuarTransferencia;

internal sealed class EfetuarTransferenciaCommandHandler(
    ITransferenciaRepository transferenciaRepository,
    IContaCorrenteService contaCorrenteService)
    : IRequestHandler<EfetuarTransferenciaCommand, EfetuarTransferenciaResponse>
{
    public async Task<EfetuarTransferenciaResponse> Handle(
        EfetuarTransferenciaCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Valor <= 0)
            return EfetuarTransferenciaResponse.ValorInvalido("Apenas valores positivos podem ser transferidos.");

        var contaOrigem = await contaCorrenteService.GetByIdAsync(
            request.IdContaCorrenteOrigem,
            request.Token,
            cancellationToken);

        if (contaOrigem is null)
            return EfetuarTransferenciaResponse.ContaInvalida("A conta corrente autenticada e invalida.");

        if (!contaOrigem.Ativa)
            return EfetuarTransferenciaResponse.ContaInativa("A conta corrente autenticada está inativa.");

        var contaDestino = await contaCorrenteService.GetByNumberAsync(
            request.NumeroContaDestino,
            request.Token,
            cancellationToken);

        if (contaDestino is null)
            return EfetuarTransferenciaResponse.FalhaRequisicao(
                "INVALID_ACCOUNT",
                "A conta corrente de destino informada é inválida.");

        var debito = await contaCorrenteService.DebitAsync(
            request.Token,
            request.IdentificacaoRequisicao,
            request.Valor,
            cancellationToken);

        if (!debito.Success)
            return EfetuarTransferenciaResponse.FalhaRequisicao(debito.TipoFalha!, debito.Mensagem!);

        var credito = await contaCorrenteService.CreditAsync(
            request.Token,
            request.IdentificacaoRequisicao,
            request.NumeroContaDestino,
            request.Valor,
            cancellationToken);

        if (!credito.Success)
        {
            var estorno = await contaCorrenteService.ReverseAsync(
                request.Token,
                request.IdentificacaoRequisicao,
                request.Valor,
                cancellationToken);

            if (!estorno.Success)
            {
                return EfetuarTransferenciaResponse.FalhaRequisicao(
                    estorno.TipoFalha ?? "TRANSFER_ROLLBACK_FAILED",
                    $"{credito.Mensagem} O estorno automático da conta de origem também falhou: {estorno.Mensagem}");
            }

            return EfetuarTransferenciaResponse.FalhaRequisicao(credito.TipoFalha!, credito.Mensagem!);
        }

        var transferencia = TransferenciaEntity.Criar(
            request.IdContaCorrenteOrigem,
            contaDestino.Id,
            request.Valor);

        await transferenciaRepository.CreateAsync(transferencia, cancellationToken);

        return EfetuarTransferenciaResponse.Sucesso();
    }
}
