using ContaCorrente.Domain.ValueObjects;

namespace ContaCorrente.Domain.Entities;

public sealed class ContaCorrente
{
    private ContaCorrente(Guid id, NumeroConta numeroConta, Cpf cpf, Senha senha, string nome, bool ativo)
    {
        Id = id;
        NumeroConta = numeroConta;
        Cpf = cpf;
        Senha = senha;
        Nome = nome;
        Ativo = ativo;
    }

    public Guid Id { get; }

    public NumeroConta NumeroConta { get; }

    public Cpf Cpf { get; }

    public Senha Senha { get; }

    public string Nome { get; }

    public bool Ativo { get; private set; }

    public static ContaCorrente Criar(NumeroConta numeroConta, Cpf cpf, string senha) =>
        new(Guid.NewGuid(), numeroConta, cpf, Senha.Criar(senha), string.Empty, true);

    public static ContaCorrente Restaurar(Guid id, NumeroConta numeroConta, Cpf cpf, Senha senha, string nome, bool ativo) =>
        new(id, numeroConta, cpf, senha, nome, ativo);

    public bool Autenticar(string senha) => Ativo && Senha.Verificar(senha);

    public void Inativar() => Ativo = false;
}