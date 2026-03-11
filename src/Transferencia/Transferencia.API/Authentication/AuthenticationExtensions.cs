using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Transferencia.API.Authentication;

internal static class AuthenticationExtensions
{
    internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = new JwtOptions
        {
            Issuer = configuration[$"{JwtOptions.SectionName}:Issuer"] ?? string.Empty,
            Audience = configuration[$"{JwtOptions.SectionName}:Audience"] ?? string.Empty,
            SigningKey = configuration[$"{JwtOptions.SectionName}:SigningKey"] ?? string.Empty,
            ExpirationInMinutes = int.TryParse(
                configuration[$"{JwtOptions.SectionName}:ExpirationInMinutes"],
                out var expirationInMinutes)
                ? expirationInMinutes
                : 60,
        };

        if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey))
            throw new InvalidOperationException("A chave de assinatura do JWT não foi configurada.");

        services.AddSingleton(jwtOptions);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    },
                };
            });

        services.AddAuthorization();

        return services;
    }
}

internal sealed class JwtOptions
{
    internal const string SectionName = "Jwt";
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SigningKey { get; init; } = string.Empty;
    public int ExpirationInMinutes { get; init; } = 60;
}
