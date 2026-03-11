using ContaCorrente.Application.Commands.DepositarContaCorrente;

namespace ContaCorrente.Tests.Application.Commands.DepositarContaCorrente;

public sealed class DepositarContaCorrenteCommandTests
{
    [Fact]
    public void Command_DeveArmazenarValoresInformados()
    {
        var idContaCorrente = Guid.NewGuid();

        var command = new DepositarContaCorrenteCommand(
            idContaCorrente,
            "req-123",
            100001,
            150.75m);

        Assert.Equal(idContaCorrente, command.IdContaCorrenteAutenticada);
        Assert.Equal("req-123", command.IdentificacaoRequisicao);
        Assert.Equal(100001, command.NumeroConta);
        Assert.Equal(150.75m, command.Valor);
    }

    [Fact]
    public void Response_Sucesso_DeveRetornarSucesso()
    {
        var response = DepositarContaCorrenteResponse.Sucesso();

        Assert.True(response.Success);
        Assert.Null(response.TipoFalha);
        Assert.Null(response.Mensagem);
    }

    [Fact]
    public void Response_ContaInvalida_DeveRetornarFalhaEsperada()
    {
        var response = DepositarContaCorrenteResponse.ContaInvalida("conta inválida");

        Assert.False(response.Success);
        Assert.Equal("INVALID_ACCOUNT", response.TipoFalha);
        Assert.Equal("conta inválida", response.Mensagem);
    }

    [Fact]
    public void Response_ContaInativa_DeveRetornarFalhaEsperada()
    {
        var response = DepositarContaCorrenteResponse.ContaInativa("conta inativa");

        Assert.False(response.Success);
        Assert.Equal("INACTIVE_ACCOUNT", response.TipoFalha);
        Assert.Equal("conta inativa", response.Mensagem);
    }

    [Fact]
    public void Response_ValorInvalido_DeveRetornarFalhaEsperada()
    {
        var response = DepositarContaCorrenteResponse.ValorInvalido("valor inválido");

        Assert.False(response.Success);
        Assert.Equal("INVALID_VALUE", response.TipoFalha);
        Assert.Equal("valor inválido", response.Mensagem);
    }
}
