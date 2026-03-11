using Dapper;
using System.Globalization;
using Tarifas.Domain.Repositories;
using TarifaEntity = Tarifas.Domain.Entities.Tarifa;

namespace Tarifas.Infrastructure.Persistence.Repositories;

internal sealed class TarifaRepository(SqliteConnectionFactory connectionFactory) : ITarifaRepository
{
    public async Task CreateAsync(TarifaEntity tarifa, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        const string sql = """
                           INSERT INTO tarifa (idtarifa, idcontacorrente, valor, datahoratarifacao)
                           VALUES (@IdTarifa, @IdContaCorrente, @Valor, @DataHoraTarifacao);
                           """;

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    IdTarifa = tarifa.Id,
                    tarifa.IdContaCorrente,
                    tarifa.Valor,
                    DataHoraTarifacao = tarifa.DataHoraTarifacao.ToString("O", CultureInfo.InvariantCulture),
                },
                cancellationToken: cancellationToken
            )
        );
    }
}
