using System.Text.Json.Serialization;
using ContaCorrente.API.Serialization;
using ContaCorrente.Domain.Enums;

namespace ContaCorrente.API.Contracts;

public sealed class MovimentarContaCorrenteRequest
{
    public int? NumeroConta { get; init; }
    public decimal Valor { get; init; }

    [JsonConverter(typeof(TipoMovimentoJsonConverter))]
    public TipoMovimento TipoMovimento { get; init; }
}
