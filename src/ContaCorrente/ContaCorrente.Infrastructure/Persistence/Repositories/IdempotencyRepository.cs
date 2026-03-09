using Dapper;
using SharedKernel.Idempotency;

namespace ContaCorrente.Infrastructure.Persistence.Repositories;

internal sealed class IdempotencyRepository(SqliteConnectionFactory connectionFactory) : IIdempotencyRepository
{
    public async Task<IdempotencyEntry?> GetAsync(string key, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        const string sql = """
                           SELECT chave_idempotencia AS Key,
                                  requisicao AS Request,
                                  resultado AS Result
                           FROM idempotencia
                           WHERE chave_idempotencia = @Key
                           LIMIT 1;
                           """;

        var data = await connection.QuerySingleOrDefaultAsync<IdempotencyData>(
            new CommandDefinition(
                sql,
                new { Key = key },
                cancellationToken: cancellationToken
            )
        );

        return data is null ? null : new IdempotencyEntry(data.Key, data.Request, data.Result);
    }

    public async Task<bool> TryCreateAsync(string key, string request, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        const string sql = """
                           INSERT OR IGNORE INTO idempotencia (chave_idempotencia, requisicao, resultado)
                           VALUES (@Key, @Request, NULL);
                           """;

        var rowsAffected = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    Key = key,
                    Request = request,
                },
                cancellationToken: cancellationToken
            )
        );

        return rowsAffected > 0;
    }

    public async Task SaveResultAsync(string key, string result, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        const string sql = """
                           UPDATE idempotencia
                           SET resultado = @Result
                           WHERE chave_idempotencia = @Key;
                           """;

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    Key = key,
                    Result = result,
                },
                cancellationToken: cancellationToken
            )
        );
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                "DELETE FROM idempotencia WHERE chave_idempotencia = @Key;",
                new { Key = key },
                cancellationToken: cancellationToken
            )
        );
    }

    private sealed record IdempotencyData(string Key, string Request, string? Result);
}
