using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Tarifas.Infrastructure.Persistence;

public sealed class SqliteConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(IConfiguration configuration) =>
        _connectionString = configuration.GetConnectionString("Default")!;

    public SqliteConnection Create() =>
        new(_connectionString);
}
