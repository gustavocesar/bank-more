using Microsoft.Extensions.DependencyInjection;

namespace Transferencia.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddTransferenciaApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}