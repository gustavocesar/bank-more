namespace ContaCorrente.Domain.ValueObjects;

public sealed record NumeroConta
{
    private const int NumeroBase = 100000;

    private NumeroConta(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public static NumeroConta Criar(int value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, NumeroBase);

        return new NumeroConta(value);
    }

    public static NumeroConta GerarProximo(int? ultimoNumero) =>
        Criar((ultimoNumero ?? NumeroBase) + 1);
}
