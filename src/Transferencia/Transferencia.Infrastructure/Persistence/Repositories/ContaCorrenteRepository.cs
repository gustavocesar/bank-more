using Dapper;
using Transferencia.Domain.Repositories;
using TransferenciaEntity = Transferencia.Domain.Entities.Transferencia;

namespace Transferencia.Infrastructure.Persistence.Repositories;

internal sealed class TransferenciaRepository(SqliteConnectionFactory connectionFactory) : ITransferenciaRepository
{
    public async Task CreateAsync(TransferenciaEntity transferencia, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        const string sql = """
                           INSERT INTO transferencia (
                               idtransferencia,
                               idcontacorrente_origem,
                               idcontacorrente_destino,
                               datamovimento,
                               valor
                           )
                           VALUES (
                               @IdTransferencia,
                               @IdContaCorrenteOrigem,
                               @IdContaCorrenteDestino,
                               @DataMovimento,
                               @Valor
                           );
                           """;

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    IdTransferencia = transferencia.Id,
                    IdContaCorrenteOrigem = transferencia.IdContaCorrenteOrigem,
                    IdContaCorrenteDestino = transferencia.IdContaCorrenteDestino,
                    DataMovimento = transferencia.DataMovimento.ToString("dd/MM/yyyy"),
                    transferencia.Valor,
                },
                cancellationToken: cancellationToken));
    }
}