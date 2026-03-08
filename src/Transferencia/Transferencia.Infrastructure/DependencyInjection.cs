using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
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
        services
            .AddHttpClient<IContaCorrenteService, ContaCorrenteService>((serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<ContaCorrenteApiOptions>();
                httpClient.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
            })
            .AddPolicyHandler((serviceProvider, _) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<ContaCorrenteService>>();
                return CreateRetryPolicy(logger);
            });

        return services;
    }

    private static AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy(ILogger<ContaCorrenteService> logger) =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(retryAttempt),
                (outcome, timespan, retryAttempt, _) =>
                {
                    logger.LogWarning(
                        "Retry {RetryAttempt} ao chamar a API Conta Corrente. Aguardando {Delay} antes da nova tentativa. StatusCode: {StatusCode}",
                        retryAttempt,
                        timespan,
                        outcome.Result?.StatusCode
                    );
                }
            );
}