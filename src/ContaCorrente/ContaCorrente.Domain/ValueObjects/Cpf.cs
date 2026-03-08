using SharedKernel.Extensions;

namespace ContaCorrente.Domain.ValueObjects;

public sealed record Cpf
{
    public string Value { get; }

    private Cpf(string value)
    {
        Value = value;
    }

    public static bool TryCreate(string? value, out Cpf? cpf)
    {
        var normalizedValue = value.OnlyNumbers();

        if (!IsValid(normalizedValue))
        {
            cpf = null;
            return false;
        }

        cpf = new Cpf(normalizedValue);
        return true;
    }

    public override string ToString() => Value;

    private static bool IsValid(string value)
    {
        if (value.Length is not 11 || value.Distinct().Count() is 1)
            return false;

        var digits = value.Select(character => character - '0').ToArray();
        var firstVerifier = CalculateVerifierDigit(digits, 9);
        var secondVerifier = CalculateVerifierDigit(digits, 10);

        return digits[9] == firstVerifier && digits[10] == secondVerifier;
    }

    private static int CalculateVerifierDigit(IReadOnlyList<int> digits, int length)
    {
        var factor = length + 1;
        var sum = 0;

        for (var index = 0; index < length; index++)
            sum += digits[index] * (factor - index);

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }
}