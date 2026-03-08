using ContaCorrente.Domain.Repositories;
using ContaCorrente.Domain.ValueObjects;
using ContaCorrenteEntity = ContaCorrente.Domain.Entities.ContaCorrente;
using Dapper;
using SharedKernel.Extensions;

namespace ContaCorrente.Infrastructure.Persistence.Repositories;

internal sealed class ContaCorrenteRepository(SqliteConnectionFactory connectionFactory) : IContaCorrenteRepository
{
    public async Task<bool> ExistsByCpfAsync(Cpf cpf, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        const string sql = """
                           SELECT EXISTS(
                               SELECT 1
                               FROM contacorrente
                               WHERE cpf = @Cpf
                           );
                           """;

        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                sql,
                new { Cpf = cpf.Value },
                cancellationToken: cancellationToken));
    }

    public async Task<int?> GetLastNumberAsync(CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<int?>(
            new CommandDefinition(
                "SELECT MAX(numero) FROM contacorrente;",
                cancellationToken: cancellationToken));
    }

    public async Task CreateAsync(ContaCorrenteEntity contaCorrente, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        const string sql = """
                           INSERT INTO contacorrente (idcontacorrente, numero, cpf, nome, ativo, senha, salt)
                           VALUES (@IdContaCorrente, @NumeroConta, @Cpf, @Nome, @Ativo, @Senha, @Salt);
                           """;

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    IdContaCorrente = contaCorrente.Id,
                    NumeroConta = contaCorrente.NumeroConta.Value,
                    Cpf = contaCorrente.Cpf.Value,
                    Nome = contaCorrente.Nome,
                    Ativo = contaCorrente.Ativo,
                    Senha = contaCorrente.Senha.Hash,
                    Salt = contaCorrente.Senha.Salt,
                },
                cancellationToken: cancellationToken));
    }

    public async Task<ContaCorrenteEntity?> GetByNumeroOrCpfAsync(string identificador, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        const string sql = """
                           SELECT idcontacorrente AS IdContaCorrente,
                                  numero AS Numero,
                                  cpf AS Cpf,
                                  nome AS Nome,
                                  ativo AS Ativo,
                                  senha AS Senha,
                                  salt AS Salt
                           FROM contacorrente
                           WHERE cpf = @Identificador OR CAST(numero AS TEXT) = @Identificador
                           LIMIT 1;
                           """;

        var data = await connection.QuerySingleOrDefaultAsync<ContaCorrenteData>(
            new CommandDefinition(
                sql,
                new { Identificador = identificador.OnlyNumbers() },
                cancellationToken: cancellationToken));

        return Map(data);
    }

    public async Task<ContaCorrenteEntity?> GetByIdAsync(Guid idContaCorrente, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        const string sql = """
                           SELECT idcontacorrente AS IdContaCorrente,
                                  numero AS Numero,
                                  cpf AS Cpf,
                                  nome AS Nome,
                                  ativo AS Ativo,
                                  senha AS Senha,
                                  salt AS Salt
                           FROM contacorrente
                           WHERE idcontacorrente = @IdContaCorrente
                           LIMIT 1;
                           """;

        var data = await connection.QuerySingleOrDefaultAsync<ContaCorrenteData>(
            new CommandDefinition(
                sql,
                new { IdContaCorrente = idContaCorrente },
                cancellationToken: cancellationToken));

        return Map(data);
    }

    public async Task InactivateAsync(Guid idContaCorrente, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        const string sql = """
                           UPDATE contacorrente
                           SET ativo = 0
                           WHERE idcontacorrente = @IdContaCorrente;
                           """;

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { IdContaCorrente = idContaCorrente },
                cancellationToken: cancellationToken));
    }

    private static ContaCorrenteEntity? Map(ContaCorrenteData? data)
    {
        if (data is null ||
            !Guid.TryParse(data.IdContaCorrente, out var idContaCorrente) ||
            !Cpf.TryCreate(data.Cpf, out var cpf))
        {
            return null;
        }

        return ContaCorrenteEntity.Restaurar(
            idContaCorrente,
            NumeroConta.Criar(checked((int)data.Numero)),
            cpf!,
            Senha.Restaurar(data.Senha, data.Salt),
            data.Nome,
            data.Ativo != 0
        );
    }

    private sealed record ContaCorrenteData(
        string IdContaCorrente,
        long Numero,
        string Cpf,
        string Nome,
        long Ativo,
        string Senha,
        string Salt
    );
}