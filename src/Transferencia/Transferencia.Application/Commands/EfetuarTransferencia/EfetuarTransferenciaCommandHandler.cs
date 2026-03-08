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
        var valorInvalido = ValidarValor(request);
        if (valorInvalido is not null)
            return valorInvalido;

        var contaOrigemResult = await ObterContaOrigemValidaAsync(request, cancellationToken);
        if (contaOrigemResult.Falha is not null)
            return contaOrigemResult.Falha;

        var contaDestinoResult = await ObterContaDestinoValidaAsync(request, cancellationToken);
        if (contaDestinoResult.Falha is not null)
            return contaDestinoResult.Falha;

        var movimentacaoResult = await ExecutarMovimentacoesAsync(request, cancellationToken);
        if (movimentacaoResult is not null)
            return movimentacaoResult;

        await PersistirTransferenciaAsync(request, contaDestinoResult.ContaDestino!, cancellationToken);

        return EfetuarTransferenciaResponse.Sucesso();
    }

    private static EfetuarTransferenciaResponse? ValidarValor(EfetuarTransferenciaCommand request)
    {
        if (request.Valor <= 0)
            return EfetuarTransferenciaResponse.ValorInvalido("Apenas valores positivos podem ser transferidos.");

        return null;
    }

    private async Task<ContaOrigemResult> ObterContaOrigemValidaAsync(
        EfetuarTransferenciaCommand request,
        CancellationToken cancellationToken)
    {
        var contaOrigem = await contaCorrenteService.GetByIdAsync(
            request.IdContaCorrenteOrigem,
            request.Token,
            cancellationToken
        );

        if (contaOrigem is null)
        {
            return new ContaOrigemResult(
                null,
                EfetuarTransferenciaResponse.ContaInvalida("A conta corrente autenticada e invalida.")
            );
        }

        if (!contaOrigem.Ativa)
        {
            return new ContaOrigemResult(
                null,
                EfetuarTransferenciaResponse.ContaInativa("A conta corrente autenticada está inativa.")
            );
        }

        return new ContaOrigemResult(contaOrigem, null);
    }

    private async Task<ContaDestinoResult> ObterContaDestinoValidaAsync(
        EfetuarTransferenciaCommand request,
        CancellationToken cancellationToken)
    {
        var contaDestino = await contaCorrenteService.GetByNumberAsync(
            request.NumeroContaDestino,
            request.Token,
            cancellationToken
        );

        if (contaDestino is null)
        {
            return new ContaDestinoResult(
                null,
                EfetuarTransferenciaResponse.FalhaRequisicao(
                    "INVALID_ACCOUNT",
                    "A conta corrente de destino informada é inválida.")
            );
        }

        return new ContaDestinoResult(contaDestino, null);
    }

    private async Task<EfetuarTransferenciaResponse?> ExecutarMovimentacoesAsync(
        EfetuarTransferenciaCommand request,
        CancellationToken cancellationToken)
    {
        var debito = await contaCorrenteService.DebitAsync(
            request.Token,
            request.IdentificacaoRequisicao,
            request.Valor,
            cancellationToken
        );

        if (!debito.Success)
            return CriarFalha(debito);

        var credito = await contaCorrenteService.CreditAsync(
            request.Token,
            request.IdentificacaoRequisicao,
            request.NumeroContaDestino,
            request.Valor,
            cancellationToken
        );

        if (credito.Success)
            return null;

        return await TentarEstornarDebitoAsync(request, credito, cancellationToken);
    }

    private async Task<EfetuarTransferenciaResponse> TentarEstornarDebitoAsync(
        EfetuarTransferenciaCommand request,
        ContaCorrenteOperationResult credito,
        CancellationToken cancellationToken)
    {
        var estorno = await contaCorrenteService.ReverseAsync(
            request.Token,
            request.IdentificacaoRequisicao,
            request.Valor,
            cancellationToken
        );

        if (!estorno.Success)
        {
            return EfetuarTransferenciaResponse.FalhaRequisicao(
                estorno.TipoFalha ?? "TRANSFER_ROLLBACK_FAILED",
                $"{credito.Mensagem} O estorno automático da conta de origem também falhou: {estorno.Mensagem}"
            );
        }

        return CriarFalha(credito);
    }

    private async Task PersistirTransferenciaAsync(
        EfetuarTransferenciaCommand request,
        ContaCorrenteInfo contaDestino,
        CancellationToken cancellationToken)
    {
        var transferencia = TransferenciaEntity.Criar(
            request.IdContaCorrenteOrigem,
            contaDestino.Id,
            request.Valor
        );

        await transferenciaRepository.CreateAsync(transferencia, cancellationToken);
    }

    private static EfetuarTransferenciaResponse CriarFalha(ContaCorrenteOperationResult operacao) =>
        EfetuarTransferenciaResponse.FalhaRequisicao(operacao.TipoFalha!, operacao.Mensagem!);

    private sealed record ContaOrigemResult(ContaCorrenteInfo? ContaOrigem, EfetuarTransferenciaResponse? Falha);

    private sealed record ContaDestinoResult(ContaCorrenteInfo? ContaDestino, EfetuarTransferenciaResponse? Falha);
}
