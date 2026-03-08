using ContaCorrente.API.Authentication;
using ContaCorrente.API.Contracts;
using ContaCorrente.API.Idempotencia;
using ContaCorrente.Application.Commands.EfetuarLogin;
using MediatR;

namespace ContaCorrente.API.Endpoints;

internal static class AutenticacaoEndpoints
{
    internal static void MapAutenticacaoEndpoints(this WebApplication app)
    {
        app.MapPost("/v1/login", async (
            EfetuarLoginRequest request,
            ISender sender,
            IJwtTokenService jwtTokenService,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(
                new EfetuarLoginCommand(request.NumeroContaOuCpf, request.Senha),
                cancellationToken
            );

            if (!response.Success)
            {
                return Results.Json(
                    new FalhaResponse(response.TipoFalha!, response.Mensagem!),
                    statusCode: StatusCodes.Status401Unauthorized
                );
            }

            return Results.Ok(
                new EfetuarLoginHttpResponse(jwtTokenService.Generate(response.IdContaCorrente!.Value))
            );
        })
        .RequireIdempotency()
        .WithName("EfetuarLogin")
        .WithSummary("Autentica uma conta corrente.")
        .WithDescription("Recebe número da conta ou CPF e senha, valida as credenciais e retorna um token JWT.")
        .Produces<EfetuarLoginHttpResponse>(StatusCodes.Status200OK)
        .Produces<FalhaResponse>(StatusCodes.Status401Unauthorized)
        .Produces<FalhaResponse>(StatusCodes.Status409Conflict);
    }
}
