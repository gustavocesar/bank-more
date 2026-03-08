using System.Text.Json;
using System.Text.Json.Serialization;
using ContaCorrente.Domain.Enums;

namespace ContaCorrente.API.Serialization;

internal sealed class TipoMovimentoJsonConverter : JsonConverter<TipoMovimento>
{
    public override TipoMovimento Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString()?.Trim().ToUpperInvariant();
            return value switch
            {
                nameof(TipoMovimento.C) => TipoMovimento.C,
                nameof(TipoMovimento.D) => TipoMovimento.D,
                _ => TipoMovimento.Indefinido,
            };
        }

        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var numericValue))
        {
            return Enum.IsDefined(typeof(TipoMovimento), numericValue)
                ? (TipoMovimento)numericValue
                : TipoMovimento.Indefinido;
        }

        return TipoMovimento.Indefinido;
    }

    public override void Write(Utf8JsonWriter writer, TipoMovimento value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}
