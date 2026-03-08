using ContaCorrente.Domain.ValueObjects;

namespace ContaCorrente.Tests.Domain.ValueObjects;

public sealed class SenhaTests
{
    [Fact]
    public void Criar_DeveGerarHashESalt_EPermitirVerificacaoDaSenhaOriginal()
    {
        var senha = Senha.Criar("Senha@123");

        Assert.False(string.IsNullOrWhiteSpace(senha.Hash));
        Assert.False(string.IsNullOrWhiteSpace(senha.Salt));
        Assert.True(senha.Verificar("Senha@123"));
        Assert.False(senha.Verificar("SenhaErrada"));
    }

    [Fact]
    public void Restaurar_DevePermitirVerificacaoComHashESaltExistentes()
    {
        var senhaOriginal = Senha.Criar("Senha@123");

        var senhaRestaurada = Senha.Restaurar(senhaOriginal.Hash, senhaOriginal.Salt);

        Assert.True(senhaRestaurada.Verificar("Senha@123"));
        Assert.False(senhaRestaurada.Verificar("OutraSenha"));
        Assert.Equal(senhaOriginal.Hash, senhaRestaurada.Hash);
        Assert.Equal(senhaOriginal.Salt, senhaRestaurada.Salt);
    }
}
