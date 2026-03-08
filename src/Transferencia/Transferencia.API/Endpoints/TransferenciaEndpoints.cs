using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Transferencia.API.Contracts;
using Transferencia.API.Idempotency;
using Transferencia.Application.Commands.EfetuarTransferencia;

namespace Transferencia.API.Endpoints;

internal static class TransferenciaEndpoints
{
    internal static void MapTransferenciaEndpoints(this WebApplication app)
    {
        EfetuarTransferencia(app);
    }

    private static void EfetuarTransferencia(WebApplication app)
    {
        app.MapPost("/v1/transferencias", async (
            ClaimsPrincipal user,
            [FromHeader(Name = "Idempotency-Key")] string identificacaoRequisicao,
            EfetuarTransferenciaRequest request,
            HttpContext httpContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var idContaCorrente = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idContaCorrente, out var contaCorrenteId))
                return Results.StatusCode(StatusCodes.Status403Forbidden);

            var token = httpContext.Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(token))
                return Results.StatusCode(StatusCodes.Status403Forbidden);

            var response = await sender.Send(
                new EfetuarTransferenciaCommand(
                    contaCorrenteId,
                    identificacaoRequisicao,
                    request.NumeroContaDestino,
                    request.Valor,
                    token),
                cancellationToken);

            if (!response.Success)
                return Results.BadRequest(new FalhaResponse(response.TipoFalha!, response.Mensagem!));

            return Results.NoContent();
        })
        .RequireAuthorization()
        .RequireIdempotency()
        .WithName("EfetuarTransferencia")
        .WithSummary("Efetua uma transferencia entre contas da mesma instituicao.")
        .WithDescription("Recebe a conta de destino e o valor, valida a conta autenticada, realiza debito e credito na API Conta Corrente e persiste a transferencia.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<FalhaResponse>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces<FalhaResponse>(StatusCodes.Status409Conflict);
    }
}