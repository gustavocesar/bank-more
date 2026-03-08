using ContaCorrente.Application.Queries.ObterContaCorrentePorId;
using ContaCorrente.Domain.ValueObjects;
using ContaCorrente.Tests.TestDoubles;
using ContaCorrenteEntity = ContaCorrente.Domain.Entities.ContaCorrente;

namespace ContaCorrente.Tests.Application.Queries.ObterContaCorrentePorId;

public sealed class ObterContaCorrentePorIdQueryHandlerTests
{
    [Fact]
    public async Task Handler_DeveRetornarContaNaoEncontrada_QuandoContaNaoExistir()
    {
        var repository = new FakeContaCorrenteRepository();
        var handler = new ObterContaCorrentePorIdQueryHandler(repository);

        var response = await handler.Handle(new ObterContaCorrentePorIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("INVALID_ACCOUNT", response.TipoFalha);
    }

    [Fact]
    public async Task Handler_DeveRetornarContaCorrente_QuandoContaExistir()
    {
        var conta = CriarConta();
        var repository = new FakeContaCorrenteRepository
        {
            ContaByIdResult = conta
        };
        var handler = new ObterContaCorrentePorIdQueryHandler(repository);

        var response = await handler.Handle(new ObterContaCorrentePorIdQuery(conta.Id), CancellationToken.None);

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
