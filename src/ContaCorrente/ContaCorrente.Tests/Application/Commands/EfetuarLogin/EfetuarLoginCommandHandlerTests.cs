using ContaCorrente.Application.Commands.EfetuarLogin;
using ContaCorrente.Domain.ValueObjects;
using ContaCorrente.Tests.TestDoubles;
using ContaCorrenteEntity = ContaCorrente.Domain.Entities.ContaCorrente;

namespace ContaCorrente.Tests.Application.Commands.EfetuarLogin;

public class EfetuarLoginCommandHandlerTests
{
    [Fact]
    public async Task Handler_DeveRetornarNaoAutorizado_QuandoContaNaoExistir()
    {
        var repository = new FakeContaCorrenteRepository();
        var handler = new EfetuarLoginCommandHandler(repository);

        var response = await handler.Handle(new EfetuarLoginCommand("100001", "Senha@123"), CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("USER_UNAUTHORIZED", response.TipoFalha);
    }

    [Fact]
    public async Task Handler_DeveRetornarNaoAutorizado_QuandoSenhaForInvalida()
    {
        var repository = new FakeContaCorrenteRepository
        {
            ContaByNumeroOrCpfResult = CriarConta()
        };
        var handler = new EfetuarLoginCommandHandler(repository);

        var response = await handler.Handle(new EfetuarLoginCommand("100001", "SenhaErrada"), CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("USER_UNAUTHORIZED", response.TipoFalha);
    }

    [Fact]
    public async Task Handler_DeveAutenticar_QuandoCredenciaisForemValidas()
    {
        var conta = CriarConta();
        var repository = new FakeContaCorrenteRepository
        {
            ContaByNumeroOrCpfResult = conta
        };
        var handler = new EfetuarLoginCommandHandler(repository);

        var response = await handler.Handle(new EfetuarLoginCommand(conta.NumeroConta.Value.ToString(), "Senha@123"), CancellationToken.None);

        Assert.True(response.Success);
        Assert.Equal(conta.Id, response.IdContaCorrente);
    }

    private static ContaCorrenteEntity CriarConta()
    {
        var numeroConta = NumeroConta.Criar(100001);
        Cpf.TryCreate("52998224725", out var cpf);

        return ContaCorrenteEntity.Criar(numeroConta, cpf!, "Senha@123");
    }
}
