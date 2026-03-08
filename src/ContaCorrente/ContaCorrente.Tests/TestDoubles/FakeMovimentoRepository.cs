using ContaCorrente.Domain.Repositories;
using MovimentoEntity = ContaCorrente.Domain.Entities.Movimento;

namespace ContaCorrente.Tests.TestDoubles;

internal sealed class FakeMovimentoRepository : IMovimentoRepository
{
    public MovimentoEntity? CreatedMovimento { get; private set; }
    public int CreateAsyncCalls { get; private set; }

    public Task CreateAsync(MovimentoEntity movimento, CancellationToken cancellationToken)
    {
        CreatedMovimento = movimento;
        CreateAsyncCalls++;
        return Task.CompletedTask;
    }
}
