using System.Globalization;
using ContaCorrente.Domain.Repositories;
using Dapper;
using MovimentoEntity = ContaCorrente.Domain.Entities.Movimento;

namespace ContaCorrente.Infrastructure.Persistence.Repositories;

internal sealed class MovimentoRepository(SqliteConnectionFactory connectionFactory) : IMovimentoRepository
{
    public async Task CreateAsync(MovimentoEntity movimento, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        const string sql = """
                           INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor)
                           VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor);
                           """;

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    IdMovimento = movimento.Id,
                    movimento.IdContaCorrente,
                    DataMovimento = movimento.DataMovimento.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    TipoMovimento = movimento.TipoMovimento.ToString(),
                    movimento.Valor,
                },
                cancellationToken: cancellationToken
            )
        );
    }

    public async Task<decimal> GetSaldoAsync(Guid idContaCorrente, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        const string sql = """
                           SELECT COALESCE(SUM(CASE tipomovimento
                               WHEN 'C' THEN valor
                               WHEN 'D' THEN -valor
                               ELSE 0
                           END), 0)
                           FROM movimento
                           WHERE idcontacorrente = @IdContaCorrente;
                           """;

        return await connection.ExecuteScalarAsync<decimal>(
            new CommandDefinition(
                sql,
                new { IdContaCorrente = idContaCorrente },
                cancellationToken: cancellationToken
            )
        );
    }
}
