using ContaCorrente.Application.Commands.DepositarContaCorrente;
using ContaCorrente.Domain.Enums;
using ContaCorrente.Domain.ValueObjects;
using ContaCorrente.Tests.TestDoubles;
using ContaCorrenteEntity = ContaCorrente.Domain.Entities.ContaCorrente;

namespace ContaCorrente.Tests.Application.Commands.DepositarContaCorrente;

public sealed class DepositarContaCorrenteCommandHandlerTests
{
    [Fact]
    public async Task Handler_DeveRetornarValorInvalido_QuandoValorNaoForPositivo()
    {
        var conta = CriarConta();
        var contaRepository = new FakeContaCorrenteRepository
        {
            ContaByIdResult = conta
        };
        var movimentoRepository = new FakeMovimentoRepository();
        var handler = new DepositarContaCorrenteCommandHandler(contaRepository, movimentoRepository);

        var response = await handler.Handle(
            new DepositarContaCorrenteCommand(conta.Id, "req-1", null, 0m),
            CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("INVALID_VALUE", response.TipoFalha);
        Assert.Equal(0, movimentoRepository.CreateAsyncCalls);
    }

    [Fact]
    public async Task Handler_DeveRetornarContaInvalida_QuandoContaNaoExistir()
    {
        var contaRepository = new FakeContaCorrenteRepository();
        var movimentoRepository = new FakeMovimentoRepository();
        var handler = new DepositarContaCorrenteCommandHandler(contaRepository, movimentoRepository);

        var response = await handler.Handle(
            new DepositarContaCorrenteCommand(Guid.NewGuid(), "req-1", null, 10m),
            CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("INVALID_ACCOUNT", response.TipoFalha);
        Assert.Equal(0, movimentoRepository.CreateAsyncCalls);
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
        var handler = new DepositarContaCorrenteCommandHandler(contaRepository, movimentoRepository);

        var response = await handler.Handle(
            new DepositarContaCorrenteCommand(conta.Id, "req-1", null, 10m),
            CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("INACTIVE_ACCOUNT", response.TipoFalha);
        Assert.Equal(0, movimentoRepository.CreateAsyncCalls);
    }

    [Fact]
    public async Task Handler_DevePersistirDeposito_QuandoContaAutenticadaForInformada()
    {
        var conta = CriarConta();
        var contaRepository = new FakeContaCorrenteRepository
        {
            ContaByIdResult = conta
        };
        var movimentoRepository = new FakeMovimentoRepository();
        var handler = new DepositarContaCorrenteCommandHandler(contaRepository, movimentoRepository);

        var response = await handler.Handle(
            new DepositarContaCorrenteCommand(conta.Id, "req-1", null, 25.50m),
            CancellationToken.None);

        Assert.True(response.Success);
        Assert.NotNull(movimentoRepository.CreatedMovimento);
        Assert.Equal(conta.Id, movimentoRepository.CreatedMovimento!.IdContaCorrente);
        Assert.Equal(TipoMovimento.C, movimentoRepository.CreatedMovimento.TipoMovimento);
        Assert.Equal(25.50m, movimentoRepository.CreatedMovimento.Valor);
        Assert.Equal(1, movimentoRepository.CreateAsyncCalls);
        Assert.Equal(0, movimentoRepository.GetSaldoAsyncCalls);
    }

    [Fact]
    public async Task Handler_DevePermitirDeposito_ParaOutraContaQuandoNumeroForInformado()
    {
        var contaDestino = CriarConta(numero: 100002);
        var contaRepository = new FakeContaCorrenteRepository
        {
            ContaByNumeroOrCpfResult = contaDestino
        };
        var movimentoRepository = new FakeMovimentoRepository();
        var handler = new DepositarContaCorrenteCommandHandler(contaRepository, movimentoRepository);

        var response = await handler.Handle(
            new DepositarContaCorrenteCommand(Guid.NewGuid(), "req-1", contaDestino.NumeroConta.Value, 80m),
            CancellationToken.None);

        Assert.True(response.Success);
        Assert.NotNull(movimentoRepository.CreatedMovimento);
        Assert.Equal(contaDestino.Id, movimentoRepository.CreatedMovimento!.IdContaCorrente);
        Assert.Equal(TipoMovimento.C, movimentoRepository.CreatedMovimento.TipoMovimento);
        Assert.Equal(80m, movimentoRepository.CreatedMovimento.Valor);
        Assert.Equal(1, movimentoRepository.CreateAsyncCalls);
        Assert.Equal(0, movimentoRepository.GetSaldoAsyncCalls);
    }

    private static ContaCorrenteEntity CriarConta(int numero = 100001, bool ativo = true)
    {
        Cpf.TryCreate("52998224725", out var cpf);

        return ContaCorrenteEntity.Restaurar(
            Guid.NewGuid(),
            NumeroConta.Criar(numero),
            cpf!,
            Senha.Criar("Senha@123"),
            string.Empty,
            ativo);
    }
}
