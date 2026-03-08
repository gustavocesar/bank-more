using ContaCorrente.Application.Commands.CriarContaCorrente;

namespace ContaCorrente.Tests.Application.Commands.CriarContaCorrente;

public sealed class CriarContaCorrenteCommandTests
{
    [Fact]
    public void Command_DeveArmazenarValoresInformados()
    {
        var command = new CriarContaCorrenteCommand("52998224725", "Senha@123");

        Assert.Equal("52998224725", command.Cpf);
        Assert.Equal("Senha@123", command.Senha);
    }

    [Fact]
    public void Response_Criada_DeveRetornarSucesso()
    {
        var response = CriarContaCorrenteResponse.Criada(100001);

        Assert.True(response.Success);
        Assert.Equal(100001, response.NumeroConta);
        Assert.Null(response.TipoFalha);
        Assert.Null(response.Mensagem);
    }

    [Fact]
    public void Response_DocumentoInvalido_DeveRetornarFalhaEsperada()
    {
        var response = CriarContaCorrenteResponse.DocumentoInvalido("cpf inválido");

        Assert.False(response.Success);
        Assert.Null(response.NumeroConta);
        Assert.Equal("INVALID_DOCUMENT", response.TipoFalha);
        Assert.Equal("cpf inválido", response.Mensagem);
    }

    [Fact]
    public void Response_ContaAtivaJaExistente_DeveRetornarFalhaEsperada()
    {
        var response = CriarContaCorrenteResponse.ContaAtivaJaExistente("conta existente");

        Assert.False(response.Success);
        Assert.Null(response.NumeroConta);
        Assert.Equal("ACTIVE_ACCOUNT_ALREADY_EXISTS", response.TipoFalha);
        Assert.Equal("conta existente", response.Mensagem);
    }
}
