using ContaCorrente.Application.Commands.InativarContaCorrente;
using ContaCorrente.Domain.ValueObjects;
using ContaCorrente.Tests.TestDoubles;
using ContaCorrenteEntity = ContaCorrente.Domain.Entities.ContaCorrente;

namespace ContaCorrente.Tests.Application.Commands.InativarContaCorrente;

public class InativarContaCorrenteCommandHandlerTests
{
    [Fact]
    public async Task Handler_DeveRetornarContaInvalida_QuandoContaNaoExistir()
    {
        var repository = new FakeContaCorrenteRepository();
        var handler = new InativarContaCorrenteCommandHandler(repository);

        var response = await handler.Handle(new InativarContaCorrenteCommand(Guid.NewGuid(), "Senha@123"), CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("INVALID_ACCOUNT", response.TipoFalha);
        Assert.Equal(0, repository.InactivateAsyncCalls);
    }

    [Fact]
    public async Task Handler_DeveRetornarSenhaInvalida_QuandoSenhaForIncorreta()
    {
        var conta = CriarConta();
        var repository = new FakeContaCorrenteRepository
        {
            ContaByIdResult = conta
        };
        var handler = new InativarContaCorrenteCommandHandler(repository);

        var response = await handler.Handle(new InativarContaCorrenteCommand(conta.Id, "SenhaErrada"), CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("INVALID_PASSWORD", response.TipoFalha);
        Assert.True(conta.Ativo);
        Assert.Equal(0, repository.InactivateAsyncCalls);
    }

    [Fact]
    public async Task Handler_DeveInativarConta_QuandoSenhaForValida()
    {
        var conta = CriarConta();
        var repository = new FakeContaCorrenteRepository
        {
            ContaByIdResult = conta
        };
        var handler = new InativarContaCorrenteCommandHandler(repository);

        var response = await handler.Handle(new InativarContaCorrenteCommand(conta.Id, "Senha@123"), CancellationToken.None);

        Assert.True(response.Success);
        Assert.False(conta.Ativo);
        Assert.Equal(conta.Id, repository.InactivatedContaId);
        Assert.Equal(1, repository.InactivateAsyncCalls);
    }

    private static ContaCorrenteEntity CriarConta()
    {
        var numeroConta = NumeroConta.Criar(100001);
        Cpf.TryCreate("52998224725", out var cpf);

        return ContaCorrenteEntity.Criar(numeroConta, cpf!, "Senha@123");
    }
}
