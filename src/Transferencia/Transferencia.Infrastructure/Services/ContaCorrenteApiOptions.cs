namespace Transferencia.Infrastructure.Services;

internal sealed class ContaCorrenteApiOptions
{
    internal const string SectionName = "ContaCorrenteApi";
    public string BaseUrl { get; init; } = string.Empty;
}
