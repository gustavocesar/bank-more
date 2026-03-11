using MediatR;

namespace Tarifas.Application.Commands.RegistrarTarifaTransferencia;

public sealed record RegistrarTarifaTransferenciaCommand(Guid IdContaCorrente, decimal ValorTarifa) : IRequest;
