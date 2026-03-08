using Dapper;

namespace ContaCorrente.Infrastructure.Persistence;

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
            Path.Combine(AppContext.BaseDirectory, "Scripts", "contacorrente.sql"),
        };

        foreach (var script in scripts)
        {
            var sql = await File.ReadAllTextAsync(script);
            await connection.ExecuteAsync(sql);
        }
    }
}