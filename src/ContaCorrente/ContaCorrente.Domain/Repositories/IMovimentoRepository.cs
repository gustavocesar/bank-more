using MovimentoEntity = ContaCorrente.Domain.Entities.Movimento;

namespace ContaCorrente.Domain.Repositories;

public interface IMovimentoRepository
{
    Task CreateAsync(MovimentoEntity movimento, CancellationToken cancellationToken);
}
