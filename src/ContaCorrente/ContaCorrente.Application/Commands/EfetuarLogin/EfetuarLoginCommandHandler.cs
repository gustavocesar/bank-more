using ContaCorrente.Domain.Repositories;
using MediatR;

namespace ContaCorrente.Application.Commands.EfetuarLogin;

internal sealed class EfetuarLoginCommandHandler(IContaCorrenteRepository contaCorrenteRepository)
    : IRequestHandler<EfetuarLoginCommand, EfetuarLoginResponse>
{
    public async Task<EfetuarLoginResponse> Handle(
        EfetuarLoginCommand request,
        CancellationToken cancellationToken)
    {
        var contaCorrente = await contaCorrenteRepository.GetByNumeroOrCpfAsync(request.NumeroContaOuCpf, cancellationToken);

        var naoAutorizado = contaCorrente is null || !contaCorrente.Ativo || !contaCorrente.Autenticar(request.Senha);
        if (naoAutorizado)
            return EfetuarLoginResponse.NaoAutorizado("Número da conta/CPF ou senha inválidos.");

        return EfetuarLoginResponse.Autenticado(contaCorrente!.Id);
    }
}
