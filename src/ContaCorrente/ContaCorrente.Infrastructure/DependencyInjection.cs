using ContaCorrente.Domain.Repositories;
using ContaCorrente.Infrastructure.Persistence;
using ContaCorrente.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Idempotency;

namespace ContaCorrente.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddContaCorrenteInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<DbInitializer>();
        services.AddSingleton<SqliteConnectionFactory>();
        services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
        services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();

        return services;
    }
}