using System.Security.Claims;
using ContaCorrente.API.Contracts;
using ContaCorrente.API.Idempotency;
using ContaCorrente.Application.Commands.CriarContaCorrente;
using ContaCorrente.Application.Commands.InativarContaCorrente;
using ContaCorrente.Application.Queries.ObterContaCorrentePorId;
using ContaCorrente.Application.Queries.ObterContaCorrentePorNumero;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContaCorrente.API.Endpoints;

internal static class ContaCorrenteEndpoints
{
    internal static void MapContaCorrenteEndpoints(this WebApplication app)
    {
        CriarContaCorrente(app);
        ObterContaCorrentePorId(app);
        ObterContaCorrentePorNumero(app);
        InativarContaCorrente(app);
    }

    private static void CriarContaCorrente(WebApplication app)
    {
        app.MapPost("/v1/contas-correntes", async (
            CriarContaCorrenteRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
                {
                    var response = await sender.Send(
                        new CriarContaCorrenteCommand(request.Cpf, request.Senha),
                        cancellationToken
                    );

                    if (!response.Success)
                        return Results.BadRequest(new FalhaResponse(response.TipoFalha!, response.Mensagem!));

                    return Results.Created(
                        $"/contas-correntes/{response.NumeroConta}",
                        new CriarContaCorrenteHttpResponse(response.NumeroConta!.Value)
                    );
                })
        .RequireIdempotency()
        .WithName("CriarContaCorrente")
        .WithSummary("Cria uma nova conta corrente.")
        .WithDescription("Recebe CPF e senha, valida o documento, cria a conta corrente e exige o cabeçalho Idempotency-Key.")
        .Produces<CriarContaCorrenteHttpResponse>(StatusCodes.Status201Created)
        .Produces<FalhaResponse>(StatusCodes.Status400BadRequest)
        .Produces<FalhaResponse>(StatusCodes.Status409Conflict);
    }

    private static void ObterContaCorrentePorId(WebApplication app)
    {
        app.MapGet("/v1/contas-correntes/{idContaCorrente:guid}", async (
            Guid idContaCorrente,
            ISender sender,
            CancellationToken cancellationToken) =>
                {
                    var response = await sender.Send(
                        new ObterContaCorrentePorIdQuery(idContaCorrente),
                        cancellationToken
                    );

                    if (!response.Success)
                        return Results.NotFound(new FalhaResponse(response.TipoFalha!, response.Mensagem!));

                    return Results.Ok(new ObterContaCorrenteHttpResponse(
                        response.IdContaCorrente!.Value,
                        response.NumeroConta!.Value,
                        response.Cpf!,
                        response.Nome!,
                        response.Ativa!.Value
                    ));
                })
        .WithName("ObterContaCorrentePorId")
        .WithSummary("Obtém os dados da conta corrente pelo identificador.")
        .WithDescription("Retorna os dados da conta corrente correspondente ao identificador informado.")
        .Produces<ObterContaCorrenteHttpResponse>(StatusCodes.Status200OK)
        .Produces<FalhaResponse>(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden)
        .RequireAuthorization();
    }

    private static void ObterContaCorrentePorNumero(WebApplication app)
    {
        app.MapGet("/v1/contas-correntes/numero/{numeroConta}", async (
            int numeroConta,
            ISender sender,
            CancellationToken cancellationToken) =>
                {
                    var response = await sender.Send(
                        new ObterContaCorrentePorNumeroQuery(numeroConta),
                        cancellationToken
                    );

                    if (!response.Success)
                    {
                        return response.TipoFalha == "INVALID_ACCOUNT_NUMBER"
                            ? Results.BadRequest(new FalhaResponse(response.TipoFalha!, response.Mensagem!))
                            : Results.NotFound(new FalhaResponse(response.TipoFalha!, response.Mensagem!));
                    }

                    return Results.Ok(new ObterContaCorrenteHttpResponse(
                        response.IdContaCorrente!.Value,
                        response.NumeroConta!.Value,
                        response.Cpf!,
                        response.Nome!,
                        response.Ativa!.Value
                    ));
                })
        .WithName("ObterContaCorrentePorNumero")
        .WithSummary("Obtém os dados da conta corrente pelo número da conta.")
        .WithDescription("Retorna os dados da conta corrente correspondente ao número informado.")
        .Produces<ObterContaCorrenteHttpResponse>(StatusCodes.Status200OK)
        .Produces<FalhaResponse>(StatusCodes.Status400BadRequest)
        .Produces<FalhaResponse>(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden)
        .RequireAuthorization();
    }

    private static void InativarContaCorrente(WebApplication app)
    {
        app.MapDelete("/v1/contas-correntes", async (
            ClaimsPrincipal user,
            [FromBody] InativarContaCorrenteRequest request,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
                {
                    var idContaCorrente = user.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!Guid.TryParse(idContaCorrente, out var contaCorrenteId))
                        return Results.StatusCode(StatusCodes.Status403Forbidden);

                    var response = await sender.Send(
                        new InativarContaCorrenteCommand(contaCorrenteId, request.Senha),
                        cancellationToken
                    );

                    if (!response.Success)
                        return Results.BadRequest(new FalhaResponse(response.TipoFalha!, response.Mensagem!));

                    return Results.NoContent();
                })
        .RequireIdempotency()
        .WithName("InativarContaCorrente")
        .WithSummary("Inativa a conta corrente autenticada.")
        .WithDescription("Recebe a senha da conta corrente autenticada, altera o status da conta para inativa e exige o cabeçalho Idempotency-Key.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<FalhaResponse>(StatusCodes.Status400BadRequest)
        .Produces<FalhaResponse>(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status403Forbidden)
        .RequireAuthorization();
    }
}
