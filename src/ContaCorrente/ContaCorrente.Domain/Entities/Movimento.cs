using ContaCorrente.Domain.Enums;

namespace ContaCorrente.Domain.Entities;

public sealed class Movimento
{
    private Movimento(Guid id, Guid idContaCorrente, DateTime dataMovimento, TipoMovimento tipoMovimento, decimal valor)
    {
        Id = id;
        IdContaCorrente = idContaCorrente;
        DataMovimento = dataMovimento;
        TipoMovimento = tipoMovimento;
        Valor = decimal.Round(valor, 2, MidpointRounding.ToEven);
    }

    public Guid Id { get; }

    public Guid IdContaCorrente { get; }

    public DateTime DataMovimento { get; }

    public TipoMovimento TipoMovimento { get; }

    public decimal Valor { get; }

    public static Movimento Criar(Guid idContaCorrente, TipoMovimento tipoMovimento, decimal valor) =>
        new(Guid.NewGuid(), idContaCorrente, DateTime.UtcNow, tipoMovimento, valor);
}
