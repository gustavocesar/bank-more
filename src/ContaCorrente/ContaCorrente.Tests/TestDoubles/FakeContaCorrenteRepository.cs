using ContaCorrente.Domain.Repositories;
using ContaCorrente.Domain.ValueObjects;
using ContaCorrenteEntity = ContaCorrente.Domain.Entities.ContaCorrente;

namespace ContaCorrente.Tests.TestDoubles;

internal sealed class FakeContaCorrenteRepository : IContaCorrenteRepository
{
    public bool ExistsByCpfResult { get; set; }
    public int? LastNumberResult { get; set; }
    public ContaCorrenteEntity? ContaByNumeroOrCpfResult { get; set; }
    public ContaCorrenteEntity? ContaByIdResult { get; set; }
    public ContaCorrenteEntity? CreatedConta { get; private set; }
    public Guid? InactivatedContaId { get; private set; }
    public int CreateAsyncCalls { get; private set; }
    public int InactivateAsyncCalls { get; private set; }

    public Task<bool> ExistsByCpfAsync(Cpf cpf, CancellationToken cancellationToken) =>
        Task.FromResult(ExistsByCpfResult);

    public Task<int?> GetLastNumberAsync(CancellationToken cancellationToken) =>
        Task.FromResult(LastNumberResult);

    public Task CreateAsync(ContaCorrenteEntity contaCorrente, CancellationToken cancellationToken)
    {
        CreatedConta = contaCorrente;
        CreateAsyncCalls++;
        return Task.CompletedTask;
    }

    public Task<ContaCorrenteEntity?> GetByNumeroOrCpfAsync(string identificador, CancellationToken cancellationToken) =>
        Task.FromResult(ContaByNumeroOrCpfResult);

    public Task<ContaCorrenteEntity?> GetByIdAsync(Guid idContaCorrente, CancellationToken cancellationToken) =>
        Task.FromResult(ContaByIdResult);

    public Task InactivateAsync(Guid idContaCorrente, CancellationToken cancellationToken)
    {
        InactivatedContaId = idContaCorrente;
        InactivateAsyncCalls++;
        return Task.CompletedTask;
    }
}
