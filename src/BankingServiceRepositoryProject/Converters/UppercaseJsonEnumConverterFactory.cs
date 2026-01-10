using System.Text.Json;
using System.Text.Json.Serialization;

namespace BankingServiceProject.RepositoryProject.Converters;

public class UppercaseJsonEnumConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type converterType = typeof(UppercaseJsonEnumConverter<>).MakeGenericType(typeToConvert);
        object converter = Activator.CreateInstance(converterType)
                      ?? throw new InvalidOperationException($"Unable to create converter for {typeToConvert}");

        return (JsonConverter)converter;
    }
}