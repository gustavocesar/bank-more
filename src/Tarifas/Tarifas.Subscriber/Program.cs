using Tarifas.Application;
using Tarifas.Infrastructure;
using Tarifas.Infrastructure.Persistence;
using Tarifas.Subscriber;
using Tarifas.Subscriber.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection(KafkaOptions.SectionName));
builder.Services.Configure<TarifaOptions>(builder.Configuration.GetSection(TarifaOptions.SectionName));
builder.Services.AddTarifasApplication();
builder.Services.AddTarifasInfrastructure();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await dbInitializer.InitializeAsync();
}

await host.RunAsync();
