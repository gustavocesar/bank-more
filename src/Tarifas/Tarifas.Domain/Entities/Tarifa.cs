namespace Tarifas.Domain.Entities;

public sealed class Tarifa
{
    private Tarifa(Guid id, Guid idContaCorrente, decimal valor, DateTime dataHoraTarifacao)
    {
        Id = id;
        IdContaCorrente = idContaCorrente;
        Valor = decimal.Round(valor, 2, MidpointRounding.ToEven);
        DataHoraTarifacao = dataHoraTarifacao;
    }

    public Guid Id { get; }

    public Guid IdContaCorrente { get; }

    public decimal Valor { get; }

    public DateTime DataHoraTarifacao { get; }

    public static Tarifa Criar(Guid idContaCorrente, decimal valor) =>
        new(Guid.NewGuid(), idContaCorrente, valor, DateTime.UtcNow);
}
