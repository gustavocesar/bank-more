using ContaCorrente.Domain.Repositories;
using MovimentoEntity = ContaCorrente.Domain.Entities.Movimento;

namespace ContaCorrente.Tests.TestDoubles;

internal sealed class FakeMovimentoRepository : IMovimentoRepository
{
    public decimal SaldoResult { get; set; }
    public MovimentoEntity? CreatedMovimento { get; private set; }
    public int CreateAsyncCalls { get; private set; }
    public int GetSaldoAsyncCalls { get; private set; }

    public Task CreateAsync(MovimentoEntity movimento, CancellationToken cancellationToken)
    {
        CreatedMovimento = movimento;
        CreateAsyncCalls++;
        return Task.CompletedTask;
    }

    public Task<decimal> GetSaldoAsync(Guid idContaCorrente, CancellationToken cancellationToken)
    {
        GetSaldoAsyncCalls++;
        return Task.FromResult(SaldoResult);
    }
}
