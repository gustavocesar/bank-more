using ContaCorrente.Application.Commands.EfetuarLogin;

namespace ContaCorrente.Tests.Application.Commands.EfetuarLogin;

public sealed class EfetuarLoginCommandTests
{
    [Fact]
    public void Command_DeveArmazenarValoresInformados()
    {
        var command = new EfetuarLoginCommand("100001", "Senha@123");

        Assert.Equal("100001", command.NumeroContaOuCpf);
        Assert.Equal("Senha@123", command.Senha);
    }

    [Fact]
    public void Response_Autenticado_DeveRetornarSucesso()
    {
        var id = Guid.NewGuid();

        var response = EfetuarLoginResponse.Autenticado(id);

        Assert.True(response.Success);
        Assert.Equal(id, response.IdContaCorrente);
        Assert.Null(response.TipoFalha);
        Assert.Null(response.Mensagem);
    }

    [Fact]
    public void Response_NaoAutorizado_DeveRetornarFalhaEsperada()
    {
        var response = EfetuarLoginResponse.NaoAutorizado("não autorizado");

        Assert.False(response.Success);
        Assert.Null(response.IdContaCorrente);
        Assert.Equal("USER_UNAUTHORIZED", response.TipoFalha);
        Assert.Equal("não autorizado", response.Mensagem);
    }
}
