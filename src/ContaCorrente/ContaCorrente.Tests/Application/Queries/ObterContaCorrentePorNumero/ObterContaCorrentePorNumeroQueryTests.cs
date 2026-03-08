using ContaCorrente.Application.Queries.ObterContaCorrentePorNumero;

namespace ContaCorrente.Tests.Application.Queries.ObterContaCorrentePorNumero;

public sealed class ObterContaCorrentePorNumeroQueryTests
{
    [Fact]
    public void Query_DeveArmazenarNumeroContaInformado()
    {
        var query = new ObterContaCorrentePorNumeroQuery(100001);

        Assert.Equal(100001, query.NumeroConta);
    }

    [Fact]
    public void Response_Encontrada_DeveRetornarSucesso()
    {
        var id = Guid.NewGuid();

        var response = ObterContaCorrentePorNumeroResponse.Encontrada(id, 100001, "52998224725", string.Empty, true);

        Assert.True(response.Success);
        Assert.Equal(id, response.IdContaCorrente);
        Assert.Equal(100001, response.NumeroConta);
        Assert.Equal("52998224725", response.Cpf);
        Assert.Equal(string.Empty, response.Nome);
        Assert.True(response.Ativa);
        Assert.Null(response.TipoFalha);
        Assert.Null(response.Mensagem);
    }

    [Fact]
    public void Response_NumeroContaInvalido_DeveRetornarFalhaEsperada()
    {
        var response = ObterContaCorrentePorNumeroResponse.NumeroContaInvalido("numero invalido");

        Assert.False(response.Success);
        Assert.Equal("INVALID_ACCOUNT_NUMBER", response.TipoFalha);
        Assert.Equal("numero invalido", response.Mensagem);
    }

    [Fact]
    public void Response_ContaNaoEncontrada_DeveRetornarFalhaEsperada()
    {
        var response = ObterContaCorrentePorNumeroResponse.ContaNaoEncontrada("conta invalida");

        Assert.False(response.Success);
        Assert.Equal("INVALID_ACCOUNT", response.TipoFalha);
        Assert.Equal("conta invalida", response.Mensagem);
    }
}
