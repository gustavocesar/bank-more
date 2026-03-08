using ContaCorrente.Domain.ValueObjects;

namespace ContaCorrente.Tests.Domain.ValueObjects;

public sealed class NumeroContaTests
{
    [Fact]
    public void Criar_DeveRetornarNumeroConta_QuandoValorForMaiorQueNumeroBase()
    {
        var numeroConta = NumeroConta.Criar(100001);

        Assert.Equal(100001, numeroConta.Value);
    }

    [Theory]
    [InlineData(100000)]
    [InlineData(99999)]
    public void Criar_DeveLancarExcecao_QuandoValorForMenorOuIgualAoNumeroBase(int valor)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NumeroConta.Criar(valor));
    }

    [Fact]
    public void GerarProximo_DeveGerarPrimeiroNumero_QuandoUltimoNumeroForNulo()
    {
        var numeroConta = NumeroConta.GerarProximo(null);

        Assert.Equal(100001, numeroConta.Value);
    }

    [Fact]
    public void GerarProximo_DeveIncrementarUltimoNumero_QuandoInformado()
    {
        var numeroConta = NumeroConta.GerarProximo(100250);

        Assert.Equal(100251, numeroConta.Value);
    }
}
