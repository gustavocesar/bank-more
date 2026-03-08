namespace Transferencia.Domain.Services;

public interface IContaCorrenteService
{
    Task<ContaCorrenteInfo?> GetByIdAsync(Guid idContaCorrente, string token, CancellationToken cancellationToken);
    Task<ContaCorrenteInfo?> GetByNumberAsync(int numeroConta, string token, CancellationToken cancellationToken);
    Task<ContaCorrenteOperationResult> DebitAsync(string token, string identificacaoRequisicao, decimal valor, CancellationToken cancellationToken);
    Task<ContaCorrenteOperationResult> CreditAsync(string token, string identificacaoRequisicao, int numeroContaDestino, decimal valor, CancellationToken cancellationToken);
    Task<ContaCorrenteOperationResult> ReverseAsync(string token, string identificacaoRequisicao, decimal valor, CancellationToken cancellationToken);
}

public sealed record ContaCorrenteInfo(Guid Id, int NumeroConta, bool Ativa);

public sealed record ContaCorrenteOperationResult(bool Success, string? TipoFalha, string? Mensagem)
{
    public static ContaCorrenteOperationResult Succeeded() => new(true, null, null);

    public static ContaCorrenteOperationResult Failed(string tipoFalha, string mensagem) =>
        new(false, tipoFalha, mensagem);
}
