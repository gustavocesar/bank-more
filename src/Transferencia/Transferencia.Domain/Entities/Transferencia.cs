namespace Transferencia.Domain.Entities;

public sealed class Transferencia
{
    private Transferencia(
        Guid id,
        Guid idContaCorrenteOrigem,
        Guid idContaCorrenteDestino,
        DateOnly dataMovimento,
        decimal valor)
    {
        Id = id;
        IdContaCorrenteOrigem = idContaCorrenteOrigem;
        IdContaCorrenteDestino = idContaCorrenteDestino;
        DataMovimento = dataMovimento;
        Valor = decimal.Round(valor, 2, MidpointRounding.ToEven);
    }

    public Guid Id { get; }

    public Guid IdContaCorrenteOrigem { get; }

    public Guid IdContaCorrenteDestino { get; }

    public DateOnly DataMovimento { get; }

    public decimal Valor { get; }

    public static Transferencia Criar(Guid idContaCorrenteOrigem, Guid idContaCorrenteDestino, decimal valor) =>
        new(Guid.NewGuid(), idContaCorrenteOrigem, idContaCorrenteDestino, DateOnly.FromDateTime(DateTime.UtcNow), valor);
}
