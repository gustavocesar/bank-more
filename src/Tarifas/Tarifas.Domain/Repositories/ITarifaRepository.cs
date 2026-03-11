using TarifaEntity = Tarifas.Domain.Entities.Tarifa;

namespace Tarifas.Domain.Repositories;

public interface ITarifaRepository
{
    Task CreateAsync(TarifaEntity tarifa, CancellationToken cancellationToken);
}
