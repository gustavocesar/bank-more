using ContaCorrente.Domain.Repositories;
using MediatR;

namespace ContaCorrente.Application.Commands.InativarContaCorrente;

internal sealed class InativarContaCorrenteCommandHandler(IContaCorrenteRepository contaCorrenteRepository)
    : IRequestHandler<InativarContaCorrenteCommand, InativarContaCorrenteResponse>
{
    public async Task<InativarContaCorrenteResponse> Handle(
        InativarContaCorrenteCommand request,
        CancellationToken cancellationToken)
    {
        var contaCorrente = await contaCorrenteRepository.GetByIdAsync(request.IdContaCorrente, cancellationToken);

        if (contaCorrente is null || !contaCorrente.Ativo)
            return InativarContaCorrenteResponse.ContaInvalida("A conta corrente informada é inválida.");

        if (!contaCorrente.Autenticar(request.Senha))
            return InativarContaCorrenteResponse.SenhaInvalida("A senha informada é inválida.");

        contaCorrente.Inativar();

        await contaCorrenteRepository.InactivateAsync(contaCorrente.Id, cancellationToken);

        return InativarContaCorrenteResponse.ContaInativada();
    }
}
