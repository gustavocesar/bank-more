using ContaCorrente.Application.Queries.ObterSaldoContaCorrente;

namespace ContaCorrente.Tests.Application.Queries.ObterSaldoContaCorrente;

public sealed class ObterSaldoContaCorrenteQueryTests
{
    [Fact]
    public void Query_DeveArmazenarIdContaCorrenteInformado()
    {
        var idContaCorrente = Guid.NewGuid();

        var query = new ObterSaldoContaCorrenteQuery(idContaCorrente);

        Assert.Equal(idContaCorrente, query.IdContaCorrente);
    }

    [Fact]
    public void Response_Sucesso_DeveRetornarDadosEsperados()
    {
        var dataHoraConsulta = DateTime.UtcNow;

        var response = ObterSaldoContaCorrenteResponse.Sucesso(100001, "Titular", dataHoraConsulta, 35.75m);

        Assert.True(response.Success);
        Assert.Equal(100001, response.NumeroConta);
        Assert.Equal("Titular", response.NomeTitular);
        Assert.Equal(dataHoraConsulta, response.DataHoraConsulta);
        Assert.Equal(35.75m, response.Saldo);
    }

    [Fact]
    public void Response_ContaInvalida_DeveRetornarFalhaEsperada()
    {
        var response = ObterSaldoContaCorrenteResponse.ContaInvalida("conta invalida");

        Assert.False(response.Success);
        Assert.Equal("INVALID_ACCOUNT", response.TipoFalha);
        Assert.Equal("conta invalida", response.Mensagem);
    }

    [Fact]
    public void Response_ContaInativa_DeveRetornarFalhaEsperada()
    {
        var response = ObterSaldoContaCorrenteResponse.ContaInativa("conta inativa");

        Assert.False(response.Success);
        Assert.Equal("INACTIVE_ACCOUNT", response.TipoFalha);
        Assert.Equal("conta inativa", response.Mensagem);
    }
}
