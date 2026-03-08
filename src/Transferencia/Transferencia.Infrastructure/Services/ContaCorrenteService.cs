using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Transferencia.Domain.Services;

namespace Transferencia.Infrastructure.Services;

internal sealed class ContaCorrenteService(HttpClient httpClient) : IContaCorrenteService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public Task<ContaCorrenteInfo?> GetByIdAsync(
        Guid idContaCorrente,
        string token,
        CancellationToken cancellationToken) =>
        GetAsync($"/v1/contas-correntes/{idContaCorrente}", token, cancellationToken);

    public Task<ContaCorrenteInfo?> GetByNumberAsync(
        int numeroConta,
        string token,
        CancellationToken cancellationToken) =>
        GetAsync($"/v1/contas-correntes/numero/{numeroConta}", token, cancellationToken);

    public Task<ContaCorrenteOperationResult> DebitAsync(
        string token,
        string identificacaoRequisicao,
        decimal valor,
        CancellationToken cancellationToken) =>
        PostAsync(
            "/internal/movimentacoes/debito",
            token,
            identificacaoRequisicao,
            new DebitoContaCorrenteRequest(valor),
            cancellationToken);

    public Task<ContaCorrenteOperationResult> CreditAsync(
        string token,
        string identificacaoRequisicao,
        int numeroContaDestino,
        decimal valor,
        CancellationToken cancellationToken) =>
        PostAsync(
            "/internal/movimentacoes/credito",
            token,
            identificacaoRequisicao,
            new CreditoContaCorrenteRequest(numeroContaDestino, valor),
            cancellationToken);

    public Task<ContaCorrenteOperationResult> ReverseAsync(
        string token,
        string identificacaoRequisicao,
        decimal valor,
        CancellationToken cancellationToken) =>
        PostAsync(
            "/internal/movimentacoes/estorno",
            token,
            identificacaoRequisicao,
            new EstornoContaCorrenteRequest(valor),
            cancellationToken);

    private async Task<ContaCorrenteInfo?> GetAsync(
        string route,
        string token,
        CancellationToken cancellationToken)
    {
        using var request = CreateRequest(HttpMethod.Get, route, token);
        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ContaCorrenteInfo>(JsonOptions, cancellationToken);
    }

    private async Task<ContaCorrenteOperationResult> PostAsync(
        string route,
        string token,
        string identificacaoRequisicao,
        object payload,
        CancellationToken cancellationToken)
    {
        using var request = CreateRequest(HttpMethod.Post, route, token, identificacaoRequisicao);
        request.Content = JsonContent.Create(payload);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
            return ContaCorrenteOperationResult.Succeeded();

        var falha = await response.Content.ReadFromJsonAsync<FalhaContaCorrenteResponse>(JsonOptions, cancellationToken);
        return ContaCorrenteOperationResult.Failed(
            falha?.TipoFalha ?? "REQUEST_FAILURE",
            falha?.Mensagem ?? "Falha ao processar a requisicao na API Conta Corrente.");
    }

    private static HttpRequestMessage CreateRequest(
        HttpMethod method,
        string route,
        string token,
        string? identificacaoRequisicao = null)
    {
        var request = new HttpRequestMessage(method, route);
        request.Headers.TryAddWithoutValidation("Authorization", token);

        if (!string.IsNullOrWhiteSpace(identificacaoRequisicao))
            request.Headers.TryAddWithoutValidation("Idempotency-Key", identificacaoRequisicao);

        return request;
    }

    private sealed record FalhaContaCorrenteResponse(string TipoFalha, string Mensagem);
    private sealed record DebitoContaCorrenteRequest(decimal Valor);
    private sealed record CreditoContaCorrenteRequest(int NumeroContaDestino, decimal Valor);
    private sealed record EstornoContaCorrenteRequest(decimal Valor);
}
