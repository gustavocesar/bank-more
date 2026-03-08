using ContaCorrente.Application.Queries.ObterContaCorrentePorNumero;
using ContaCorrente.Domain.ValueObjects;
using ContaCorrente.Tests.TestDoubles;
using ContaCorrenteEntity = ContaCorrente.Domain.Entities.ContaCorrente;

namespace ContaCorrente.Tests.Application.Queries.ObterContaCorrentePorNumero;

public sealed class ObterContaCorrentePorNumeroQueryHandlerTests
{
    [Fact]
    public async Task Handler_DeveRetornarNumeroContaInvalido_QuandoNumeroForInvalido()
    {
        var repository = new FakeContaCorrenteRepository();
        var handler = new ObterContaCorrentePorNumeroQueryHandler(repository);

        var response = await handler.Handle(new ObterContaCorrentePorNumeroQuery(123), CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("INVALID_ACCOUNT_NUMBER", response.TipoFalha);
    }

    [Fact]
    public async Task Handler_DeveRetornarContaNaoEncontrada_QuandoContaNaoExistir()
    {
        var repository = new FakeContaCorrenteRepository();
        var handler = new ObterContaCorrentePorNumeroQueryHandler(repository);

        var response = await handler.Handle(new ObterContaCorrentePorNumeroQuery(100001), CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("INVALID_ACCOUNT", response.TipoFalha);
    }

    [Fact]
    public async Task Handler_DeveRetornarContaCorrente_QuandoContaExistir()
    {
        var conta = CriarConta();
        var repository = new FakeContaCorrenteRepository
        {
            ContaByNumeroOrCpfResult = conta
        };
        var handler = new ObterContaCorrentePorNumeroQueryHandler(repository);

        var response = await handler.Handle(new ObterContaCorrentePorNumeroQuery(conta.NumeroConta.Value), CancellationToken.None);

        Assert.True(response.Success);
        Assert.Equal(conta.Id, response.IdContaCorrente);
        Assert.Equal(conta.NumeroConta.Value, response.NumeroConta);
        Assert.Equal(conta.Cpf.Value, response.Cpf);
        Assert.Equal(conta.Nome, response.Nome);
        Assert.Equal(conta.Ativo, response.Ativa);
    }

    private static ContaCorrenteEntity CriarConta()
    {
        var numeroConta = NumeroConta.Criar(100001);
        Cpf.TryCreate("52998224725", out var cpf);

        return ContaCorrenteEntity.Criar(numeroConta, cpf!, "Senha@123");
    }
}
