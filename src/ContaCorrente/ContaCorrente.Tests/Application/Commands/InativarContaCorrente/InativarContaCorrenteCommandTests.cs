using ContaCorrente.Application.Commands.InativarContaCorrente;

namespace ContaCorrente.Tests.Application.Commands.InativarContaCorrente;

public sealed class InativarContaCorrenteCommandTests
{
    [Fact]
    public void Command_DeveArmazenarValoresInformados()
    {
        var idContaCorrente = Guid.NewGuid();

        var command = new InativarContaCorrenteCommand(idContaCorrente, "Senha@123");

        Assert.Equal(idContaCorrente, command.IdContaCorrente);
        Assert.Equal("Senha@123", command.Senha);
    }

    [Fact]
    public void Response_ContaInativada_DeveRetornarSucesso()
    {
        var response = InativarContaCorrenteResponse.ContaInativada();

        Assert.True(response.Success);
        Assert.Null(response.TipoFalha);
        Assert.Null(response.Mensagem);
    }

    [Fact]
    public void Response_ContaInvalida_DeveRetornarFalhaEsperada()
    {
        var response = InativarContaCorrenteResponse.ContaInvalida("conta inválida");

        Assert.False(response.Success);
        Assert.Equal("INVALID_ACCOUNT", response.TipoFalha);
        Assert.Equal("conta inválida", response.Mensagem);
    }

    [Fact]
    public void Response_SenhaInvalida_DeveRetornarFalhaEsperada()
    {
        var response = InativarContaCorrenteResponse.SenhaInvalida("senha inválida");

        Assert.False(response.Success);
        Assert.Equal("INVALID_PASSWORD", response.TipoFalha);
        Assert.Equal("senha inválida", response.Mensagem);
    }
}
