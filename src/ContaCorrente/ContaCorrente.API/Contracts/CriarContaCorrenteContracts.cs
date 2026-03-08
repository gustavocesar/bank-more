namespace ContaCorrente.API.Contracts;

public sealed class CriarContaCorrenteRequest
{
    public required string Cpf { get; init; }
    public required string Senha { get; init; }
}

public sealed record CriarContaCorrenteHttpResponse(int NumeroConta);
