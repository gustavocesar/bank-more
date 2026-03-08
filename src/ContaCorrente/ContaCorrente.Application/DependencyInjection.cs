using Microsoft.Extensions.DependencyInjection;

namespace ContaCorrente.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddContaCorrenteApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}