namespace ContaCorrente.API.Contracts;

public sealed record ObterContaCorrenteHttpResponse(Guid Id, int NumeroConta, string Cpf, string Nome, bool Ativa);
