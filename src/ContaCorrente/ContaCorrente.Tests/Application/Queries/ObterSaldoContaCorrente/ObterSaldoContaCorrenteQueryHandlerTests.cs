using ContaCorrente.Application.Queries.ObterSaldoContaCorrente;
using ContaCorrente.Domain.ValueObjects;
using ContaCorrente.Tests.TestDoubles;
using ContaCorrenteEntity = ContaCorrente.Domain.Entities.ContaCorrente;

namespace ContaCorrente.Tests.Application.Queries.ObterSaldoContaCorrente;

public sealed class ObterSaldoContaCorrenteQueryHandlerTests
{
    [Fact]
    public async Task Handler_DeveRetornarContaInvalida_QuandoContaNaoExistir()
    {
        var contaRepository = new FakeContaCorrenteRepository();
        var movimentoRepository = new FakeMovimentoRepository();
        var handler = new ObterSaldoContaCorrenteQueryHandler(contaRepository, movimentoRepository);

        var response = await handler.Handle(new ObterSaldoContaCorrenteQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("INVALID_ACCOUNT", response.TipoFalha);
        Assert.Equal(0, movimentoRepository.GetSaldoAsyncCalls);
    }

    [Fact]
    public async Task Handler_DeveRetornarContaInativa_QuandoContaEstiverInativa()
    {
        var conta = CriarConta(ativo: false);
        var contaRepository = new FakeContaCorrenteRepository
        {
            ContaByIdResult = conta
        };
        var movimentoRepository = new FakeMovimentoRepository();
        var handler = new ObterSaldoContaCorrenteQueryHandler(contaRepository, movimentoRepository);

        var response = await handler.Handle(new ObterSaldoContaCorrenteQuery(conta.Id), CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("INACTIVE_ACCOUNT", response.TipoFalha);
        Assert.Equal(0, movimentoRepository.GetSaldoAsyncCalls);
    }

    [Fact]
    public async Task Handler_DeveRetornarSaldoZero_QuandoContaNaoPossuirMovimentacoes()
    {
        var conta = CriarConta();
        var contaRepository = new FakeContaCorrenteRepository
        {
            ContaByIdResult = conta
        };
        var movimentoRepository = new FakeMovimentoRepository
        {
            SaldoResult = 0m
        };
        var handler = new ObterSaldoContaCorrenteQueryHandler(contaRepository, movimentoRepository);

        var response = await handler.Handle(new ObterSaldoContaCorrenteQuery(conta.Id), CancellationToken.None);

        Assert.True(response.Success);
        Assert.Equal(conta.NumeroConta.Value, response.NumeroConta);
        Assert.Equal(conta.Nome, response.NomeTitular);
        Assert.Equal(0m, response.Saldo);
        Assert.NotNull(response.DataHoraConsulta);
        Assert.Equal(1, movimentoRepository.GetSaldoAsyncCalls);
    }

    [Fact]
    public async Task Handler_DeveRetornarSaldoAtual_QuandoContaPossuirMovimentacoes()
    {
        var conta = CriarConta(nome: "Titular");
        var contaRepository = new FakeContaCorrenteRepository
        {
            ContaByIdResult = conta
        };
        var movimentoRepository = new FakeMovimentoRepository
        {
            SaldoResult = 125.4m
        };
        var handler = new ObterSaldoContaCorrenteQueryHandler(contaRepository, movimentoRepository);

        var response = await handler.Handle(new ObterSaldoContaCorrenteQuery(conta.Id), CancellationToken.None);

        Assert.True(response.Success);
        Assert.Equal(conta.NumeroConta.Value, response.NumeroConta);
        Assert.Equal("Titular", response.NomeTitular);
        Assert.Equal(125.4m, response.Saldo);
        Assert.Equal(1, movimentoRepository.GetSaldoAsyncCalls);
    }

    private static ContaCorrenteEntity CriarConta(int numero = 100001, bool ativo = true, string nome = "")
    {
        Cpf.TryCreate("52998224725", out var cpf);

        return ContaCorrenteEntity.Restaurar(
            Guid.NewGuid(),
            NumeroConta.Criar(numero),
            cpf!,
            Senha.Criar("Senha@123"),
            nome,
            ativo
        );
    }
}
