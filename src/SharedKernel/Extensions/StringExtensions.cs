namespace SharedKernel.Extensions;

public static class StringExtensions
{
    public static string OnlyNumbers(this string? value) =>
        new((value ?? string.Empty).Where(char.IsDigit).ToArray());
}
