using ContaCorrente.Domain.ValueObjects;

namespace ContaCorrente.Tests.Domain.Entities;

public sealed class ContaCorrenteTests
{
    [Fact]
    public void Criar_DeveInicializarContaAtiva_ComNomeVazioEIdPreenchido()
    {
        var numeroConta = NumeroConta.Criar(100001);
        Cpf.TryCreate("529.982.247-25", out var cpf);

        var conta = ContaCorrente.Domain.Entities.ContaCorrente.Criar(numeroConta, cpf!, "Senha@123");

        Assert.NotEqual(Guid.Empty, conta.Id);
        Assert.Equal(numeroConta, conta.NumeroConta);
        Assert.Equal(cpf, conta.Cpf);
        Assert.Equal(string.Empty, conta.Nome);
        Assert.True(conta.Ativo);
        Assert.True(conta.Autenticar("Senha@123"));
    }

    [Fact]
    public void Restaurar_DeveReconstituirEstadoDaConta()
    {
        var id = Guid.NewGuid();
        var numeroConta = NumeroConta.Criar(100001);
        Cpf.TryCreate("529.982.247-25", out var cpf);
        var senha = Senha.Criar("Senha@123");

        var conta = ContaCorrente.Domain.Entities.ContaCorrente.Restaurar(id, numeroConta, cpf!, senha, "Gustavo", true);

        Assert.Equal(id, conta.Id);
        Assert.Equal(numeroConta, conta.NumeroConta);
        Assert.Equal(cpf, conta.Cpf);
        Assert.Equal(senha, conta.Senha);
        Assert.Equal("Gustavo", conta.Nome);
        Assert.True(conta.Ativo);
    }

    [Fact]
    public void Autenticar_DeveRetornarFalse_QuandoSenhaForInvalida()
    {
        var conta = CriarConta();

        var autenticado = conta.Autenticar("SenhaErrada");

        Assert.False(autenticado);
    }

    [Fact]
    public void Autenticar_DeveRetornarFalse_QuandoContaEstiverInativa()
    {
        var conta = CriarConta();
        conta.Inativar();

        var autenticado = conta.Autenticar("Senha@123");

        Assert.False(autenticado);
    }

    [Fact]
    public void Inativar_DeveMarcarContaComoInativa()
    {
        var conta = CriarConta();

        conta.Inativar();

        Assert.False(conta.Ativo);
    }

    private static ContaCorrente.Domain.Entities.ContaCorrente CriarConta()
    {
        var numeroConta = NumeroConta.Criar(100001);
        Cpf.TryCreate("529.982.247-25", out var cpf);

        return ContaCorrente.Domain.Entities.ContaCorrente.Criar(numeroConta, cpf!, "Senha@123");
    }
}
