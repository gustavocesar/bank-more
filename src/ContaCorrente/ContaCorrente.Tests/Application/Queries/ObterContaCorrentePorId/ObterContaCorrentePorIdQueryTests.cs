using ContaCorrente.Application.Queries.ObterContaCorrentePorId;

namespace ContaCorrente.Tests.Application.Queries.ObterContaCorrentePorId;

public sealed class ObterContaCorrentePorIdQueryTests
{
    [Fact]
    public void Query_DeveArmazenarIdContaCorrenteInformado()
    {
        var idContaCorrente = Guid.NewGuid();

        var query = new ObterContaCorrentePorIdQuery(idContaCorrente);

        Assert.Equal(idContaCorrente, query.IdContaCorrente);
    }

    [Fact]
    public void Response_Encontrada_DeveRetornarSucesso()
    {
        var idContaCorrente = Guid.NewGuid();

        var response = ObterContaCorrentePorIdResponse.Encontrada(idContaCorrente, 100001, "52998224725", string.Empty, true);

        Assert.True(response.Success);
        Assert.Equal(idContaCorrente, response.IdContaCorrente);
        Assert.Equal(100001, response.NumeroConta);
        Assert.Equal("52998224725", response.Cpf);
        Assert.True(response.Ativa);
    }

    [Fact]
    public void Response_ContaNaoEncontrada_DeveRetornarFalhaEsperada()
    {
        var response = ObterContaCorrentePorIdResponse.ContaNaoEncontrada("conta invalida");

        Assert.False(response.Success);
        Assert.Equal("INVALID_ACCOUNT", response.TipoFalha);
        Assert.Equal("conta invalida", response.Mensagem);
    }
}
