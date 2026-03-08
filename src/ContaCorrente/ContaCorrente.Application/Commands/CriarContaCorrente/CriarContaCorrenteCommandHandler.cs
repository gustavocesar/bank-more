using ContaCorrente.Domain.Repositories;
using ContaCorrente.Domain.ValueObjects;
using ContaCorrenteEntity = ContaCorrente.Domain.Entities.ContaCorrente;
using MediatR;

namespace ContaCorrente.Application.Commands.CriarContaCorrente;

internal sealed class CriarContaCorrenteCommandHandler(IContaCorrenteRepository contaCorrenteRepository)
    : IRequestHandler<CriarContaCorrenteCommand, CriarContaCorrenteResponse>
{
    public async Task<CriarContaCorrenteResponse> Handle(
        CriarContaCorrenteCommand request,
        CancellationToken cancellationToken)
    {
        if (!Cpf.TryCreate(request.Cpf, out var cpf))
            return CriarContaCorrenteResponse.DocumentoInvalido("O CPF informado é inválido.");

        if (await contaCorrenteRepository.ExistsByCpfAsync(cpf!, cancellationToken))
            return CriarContaCorrenteResponse.ContaAtivaJaExistente("Já existe uma conta corrente ativa para o CPF informado.");

        var ultimoNumero = await contaCorrenteRepository.GetLastNumberAsync(cancellationToken);
        var numeroConta = NumeroConta.GerarProximo(ultimoNumero);
        var contaCorrente = ContaCorrenteEntity.Criar(numeroConta, cpf!, request.Senha);

        await contaCorrenteRepository.CreateAsync(contaCorrente, cancellationToken);

        return CriarContaCorrenteResponse.Criada(contaCorrente.NumeroConta.Value);
    }
}