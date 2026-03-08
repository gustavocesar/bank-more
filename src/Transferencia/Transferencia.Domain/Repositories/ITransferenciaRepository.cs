using TransferenciaEntity = Transferencia.Domain.Entities.Transferencia;

namespace Transferencia.Domain.Repositories;

public interface ITransferenciaRepository
{
    Task CreateAsync(TransferenciaEntity transferencia, CancellationToken cancellationToken);
}
