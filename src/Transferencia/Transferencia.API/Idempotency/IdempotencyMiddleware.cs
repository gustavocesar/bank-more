using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Primitives;
using SharedKernel.Idempotency;
using Transferencia.API.Contracts;

namespace Transferencia.API.Idempotency;

internal sealed class IdempotencyMiddleware(RequestDelegate next)
{
    private const string _idempotencyHeader = "Idempotency-Key";
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context, IIdempotencyRepository idempotencyStore)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<RequireIdempotencyAttribute>() is null)
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(_idempotencyHeader, out var idempotencyKey) || string.IsNullOrEmpty(idempotencyKey))
        {
            await WriteFailureResponseAsync(
                context,
                StatusCodes.Status400BadRequest,
                "IDEMPOTENCIA_INVALIDA",
                $"O cabeçalho '{_idempotencyHeader}' é obrigatório."
            );

            return;
        }

        var requestBody = await ReadRequestBodyAsync(context);
        var storageKey = BuildStorageKey(context, idempotencyKey.ToString());

        if (await TryHandleExistingRequestAsync(context, idempotencyStore, storageKey, requestBody))
            return;

        if (!await idempotencyStore.TryCreateAsync(storageKey, requestBody, context.RequestAborted))
        {
            if (await TryHandleExistingRequestAsync(context, idempotencyStore, storageKey, requestBody))
                return;
        }

        var originalBody = context.Response.Body;
        await using var responseBuffer = new MemoryStream();
        context.Response.Body = responseBuffer;

        try
        {
            await next(context);

            responseBuffer.Position = 0;
            using var reader = new StreamReader(
                responseBuffer,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true
            );

            var responseBody = await reader.ReadToEndAsync(context.RequestAborted);

            if (context.Response.StatusCode < StatusCodes.Status500InternalServerError)
            {
                var storedResponse = new StoredIdempotencyResponse(
                    context.Response.StatusCode,
                    context.Response.ContentType,
                    responseBody,
                    context.Response.Headers
                        .Where(header => !header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                        .ToDictionary(
                            header => header.Key,
                            header => header.Value.ToArray(),
                            StringComparer.OrdinalIgnoreCase
                        )
                );

                try
                {
                    await idempotencyStore.SaveResultAsync(
                        storageKey,
                        JsonSerializer.Serialize(storedResponse, SerializerOptions),
                        context.RequestAborted
                    );
                }
                catch
                {
                    await idempotencyStore.RemoveAsync(storageKey, context.RequestAborted);
                }
            }
            else
            {
                await idempotencyStore.RemoveAsync(storageKey, context.RequestAborted);
            }

            responseBuffer.Position = 0;
            context.Response.Body = originalBody;
            await responseBuffer.CopyToAsync(originalBody, context.RequestAborted);
        }
        catch
        {
            context.Response.Body = originalBody;
            await idempotencyStore.RemoveAsync(storageKey, context.RequestAborted);
            throw;
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    private static async Task<bool> TryHandleExistingRequestAsync(
        HttpContext context,
        IIdempotencyRepository idempotencyStore,
        string storageKey,
        string requestBody)
    {
        var existingRequest = await idempotencyStore.GetAsync(storageKey, context.RequestAborted);
        if (existingRequest is null)
            return false;

        if (!string.Equals(existingRequest.Request, requestBody, StringComparison.Ordinal))
        {
            await WriteFailureResponseAsync(
                context,
                StatusCodes.Status409Conflict,
                "IDEMPOTENCIA_CONFLITO",
                "A chave de idempotência já foi utilizada com outra requisição."
            );

            return true;
        }

        if (string.IsNullOrWhiteSpace(existingRequest.Result))
        {
            await WriteFailureResponseAsync(
                context,
                StatusCodes.Status409Conflict,
                "IDEMPOTENCIA_EM_PROCESSAMENTO",
                "Já existe uma requisição em processamento para esta chave de idempotência."
            );

            return true;
        }

        var storedResponse = JsonSerializer.Deserialize<StoredIdempotencyResponse>(existingRequest.Result, SerializerOptions);
        if (storedResponse is null)
        {
            await WriteFailureResponseAsync(
                context,
                StatusCodes.Status409Conflict,
                "IDEMPOTENCIA_INVALIDA",
                "Não foi possível reutilizar o resultado armazenado para esta chave de idempotência.");
            return true;
        }

        await WriteStoredResponseAsync(context, storedResponse);
        return true;
    }

    private static string BuildStorageKey(HttpContext context, string idempotencyKey)
    {
        var contaCorrenteId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        var rawKey = string.Join(":",
            context.Request.Method,
            context.Request.Path.Value ?? string.Empty,
            context.Request.QueryString.Value ?? string.Empty,
            contaCorrenteId,
            idempotencyKey.Trim()
        );

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawKey)));
    }

    private static async Task<string> ReadRequestBodyAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        context.Request.Body.Position = 0;

        using var reader = new StreamReader(
            context.Request.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: 1024,
            leaveOpen: true
        );

        var body = await reader.ReadToEndAsync(context.RequestAborted);
        context.Request.Body.Position = 0;
        return body;
    }

    private static async Task WriteStoredResponseAsync(HttpContext context, StoredIdempotencyResponse storedResponse)
    {
        context.Response.Clear();
        context.Response.StatusCode = storedResponse.StatusCode;

        foreach (var header in storedResponse.Headers)
            context.Response.Headers[header.Key] = new StringValues(header.Value);

        if (!string.IsNullOrWhiteSpace(storedResponse.ContentType))
            context.Response.ContentType = storedResponse.ContentType;

        if (!string.IsNullOrEmpty(storedResponse.Body))
            await context.Response.WriteAsync(storedResponse.Body, context.RequestAborted);
    }

    private static async Task WriteFailureResponseAsync(
        HttpContext context,
        int statusCode,
        string tipoFalha,
        string mensagem)
    {
        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            new FalhaResponse(tipoFalha, mensagem),
            SerializerOptions,
            context.RequestAborted
        );
    }

    private sealed record StoredIdempotencyResponse(
        int StatusCode,
        string? ContentType,
        string? Body,
        Dictionary<string, string[]> Headers
    );
}
