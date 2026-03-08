using ContaCorrente.API.Authentication;
using ContaCorrente.API.Endpoints;
using ContaCorrente.API.Idempotency;
using ContaCorrente.Application;
using ContaCorrente.Infrastructure;
using ContaCorrente.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddContaCorrenteApplication();
builder.Services.AddContaCorrenteInfrastructure();
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await initializer.InitializeAsync();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<IdempotencyMiddleware>();

app.MapContaCorrenteEndpoints();
app.MapAutenticacaoEndpoints();

app.Run();
