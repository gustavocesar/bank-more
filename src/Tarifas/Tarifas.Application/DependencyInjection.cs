using Microsoft.Extensions.DependencyInjection;

namespace Tarifas.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddTarifasApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
