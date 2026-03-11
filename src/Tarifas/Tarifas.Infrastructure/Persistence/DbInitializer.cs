using Dapper;

namespace Tarifas.Infrastructure.Persistence;

public sealed class DbInitializer
{
    private readonly SqliteConnectionFactory _factory;

    public DbInitializer(SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        using var connection = _factory.Create();

        var scripts = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Scripts", "tarifas.sql"),
        };

        foreach (var script in scripts)
        {
            var sql = await File.ReadAllTextAsync(script);
            await connection.ExecuteAsync(sql);
        }
    }
}
