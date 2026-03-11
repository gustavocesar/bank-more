using MediatR;
using Tarifas.Domain.Repositories;
using TarifaEntity = Tarifas.Domain.Entities.Tarifa;

namespace Tarifas.Application.Commands.RegistrarTarifaTransferencia;

internal sealed class RegistrarTarifaTransferenciaCommandHandler(ITarifaRepository tarifaRepository)
    : IRequestHandler<RegistrarTarifaTransferenciaCommand>
{
    public async Task Handle(RegistrarTarifaTransferenciaCommand request, CancellationToken cancellationToken)
    {
        var tarifa = TarifaEntity.Criar(request.IdContaCorrente, request.ValorTarifa);
        await tarifaRepository.CreateAsync(tarifa, cancellationToken);
    }
}
