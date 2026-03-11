using Microsoft.Extensions.DependencyInjection;
using Tarifas.Domain.Repositories;
using Tarifas.Infrastructure.Persistence;
using Tarifas.Infrastructure.Persistence.Repositories;

namespace Tarifas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTarifasInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<DbInitializer>();
        services.AddSingleton<SqliteConnectionFactory>();
        services.AddScoped<ITarifaRepository, TarifaRepository>();

        return services;
    }
}
