using ContaCorrente.Application.Commands.MovimentarContaCorrente;
using ContaCorrente.Domain.Enums;

namespace ContaCorrente.Tests.Application.Commands.MovimentarContaCorrente;

public sealed class MovimentarContaCorrenteCommandTests
{
    [Fact]
    public void Command_DeveArmazenarValoresInformados()
    {
        var idContaCorrente = Guid.NewGuid();

        var command = new MovimentarContaCorrenteCommand(
            idContaCorrente,
            "req-123",
            100001,
            150.75m,
            TipoMovimento.C);

        Assert.Equal(idContaCorrente, command.IdContaCorrenteAutenticada);
        Assert.Equal("req-123", command.IdentificacaoRequisicao);
        Assert.Equal(100001, command.NumeroConta);
        Assert.Equal(150.75m, command.Valor);
        Assert.Equal(TipoMovimento.C, command.TipoMovimento);
    }

    [Fact]
    public void Response_Sucesso_DeveRetornarSucesso()
    {
        var response = MovimentarContaCorrenteResponse.Sucesso();

        Assert.True(response.Success);
        Assert.Null(response.TipoFalha);
        Assert.Null(response.Mensagem);
    }

    [Fact]
    public void Response_ContaInvalida_DeveRetornarFalhaEsperada()
    {
        var response = MovimentarContaCorrenteResponse.ContaInvalida("conta inválida");

        Assert.False(response.Success);
        Assert.Equal("INVALID_ACCOUNT", response.TipoFalha);
        Assert.Equal("conta inválida", response.Mensagem);
    }

    [Fact]
    public void Response_ContaInativa_DeveRetornarFalhaEsperada()
    {
        var response = MovimentarContaCorrenteResponse.ContaInativa("conta inativa");

        Assert.False(response.Success);
        Assert.Equal("INACTIVE_ACCOUNT", response.TipoFalha);
        Assert.Equal("conta inativa", response.Mensagem);
    }

    [Fact]
    public void Response_ValorInvalido_DeveRetornarFalhaEsperada()
    {
        var response = MovimentarContaCorrenteResponse.ValorInvalido("valor inválido");

        Assert.False(response.Success);
        Assert.Equal("INVALID_VALUE", response.TipoFalha);
        Assert.Equal("valor inválido", response.Mensagem);
    }

    [Fact]
    public void Response_TipoInvalido_DeveRetornarFalhaEsperada()
    {
        var response = MovimentarContaCorrenteResponse.TipoInvalido("tipo inválido");

        Assert.False(response.Success);
        Assert.Equal("INVALID_TYPE", response.TipoFalha);
        Assert.Equal("tipo inválido", response.Mensagem);
    }

    [Fact]
    public void Response_SaldoInsuficiente_DeveRetornarFalhaEsperada()
    {
        var response = MovimentarContaCorrenteResponse.SaldoInsuficiente("saldo insuficiente");

        Assert.False(response.Success);
        Assert.Equal("INSUFFICIENT_FUNDS", response.TipoFalha);
        Assert.Equal("saldo insuficiente", response.Mensagem);
    }
}
