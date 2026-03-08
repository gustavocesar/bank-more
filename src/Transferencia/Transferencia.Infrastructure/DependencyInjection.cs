using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Idempotency;
using Transferencia.Domain.Repositories;
using Transferencia.Domain.Services;
using Transferencia.Infrastructure.Persistence;
using Transferencia.Infrastructure.Persistence.Repositories;
using Transferencia.Infrastructure.Services;

namespace Transferencia.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTransferenciaInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DbInitializer>();
        services.AddSingleton<SqliteConnectionFactory>();
        services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();
        services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();

        var contaCorrenteApiOptions = new ContaCorrenteApiOptions
        {
            BaseUrl = configuration[$"{ContaCorrenteApiOptions.SectionName}:BaseUrl"] ?? string.Empty,
        };

        if (string.IsNullOrWhiteSpace(contaCorrenteApiOptions.BaseUrl))
            throw new InvalidOperationException("A URL base da API Conta Corrente nao foi configurada.");

        services.AddSingleton(contaCorrenteApiOptions);
        services.AddScoped<IContaCorrenteService>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<ContaCorrenteApiOptions>();
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute),
            };

            return new ContaCorrenteService(httpClient);
        });

        return services;
    }
}