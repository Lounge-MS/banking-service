using System.Text.Json;
using System.Text.Json.Serialization;

namespace BankingServiceProject.RepositoryProject.Converters;

public class UppercaseJsonEnumConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        string s = value ?? throw new JsonException("Null value in reader is not allowed");
        return Enum.Parse<TEnum>(s, ignoreCase: true);
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        string strValue = value.ToString();
        writer.WriteStringValue(strValue.ToUpperInvariant());
    }
}