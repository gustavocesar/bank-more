using ContaCorrente.Domain.ValueObjects;

namespace ContaCorrente.Tests.Domain.ValueObjects;

public sealed class CpfTests
{
    [Fact]
    public void TryCreate_DeveCriarCpf_QuandoValorForValido()
    {
        var result = Cpf.TryCreate("529.982.247-25", out var cpf);

        Assert.True(result);
        Assert.NotNull(cpf);
        Assert.Equal("52998224725", cpf!.Value);
        Assert.Equal("52998224725", cpf.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("111.111.111-11")]
    [InlineData("529.982.247-24")]
    public void TryCreate_DeveRetornarFalse_QuandoValorForInvalido(string? valor)
    {
        var result = Cpf.TryCreate(valor, out var cpf);

        Assert.False(result);
        Assert.Null(cpf);
    }
}
