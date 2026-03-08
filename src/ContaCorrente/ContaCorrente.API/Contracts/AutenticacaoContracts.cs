namespace ContaCorrente.API.Contracts;

public sealed class EfetuarLoginRequest
{
    public required string NumeroContaOuCpf { get; init; }
    public required string Senha { get; init; }
}

public sealed record EfetuarLoginHttpResponse(string Token);