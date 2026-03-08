using ContaCorrente.Domain.ValueObjects;
using ContaCorrenteEntity = ContaCorrente.Domain.Entities.ContaCorrente;

namespace ContaCorrente.Domain.Repositories;

public interface IContaCorrenteRepository
{
    Task<bool> ExistsByCpfAsync(Cpf cpf, CancellationToken cancellationToken);
    Task<int?> GetLastNumberAsync(CancellationToken cancellationToken);
    Task CreateAsync(ContaCorrenteEntity contaCorrente, CancellationToken cancellationToken);
    Task<ContaCorrenteEntity?> GetByNumeroOrCpfAsync(string identificador, CancellationToken cancellationToken);
    Task<ContaCorrenteEntity?> GetByIdAsync(Guid idContaCorrente, CancellationToken cancellationToken);
    Task InactivateAsync(Guid idContaCorrente, CancellationToken cancellationToken);
}