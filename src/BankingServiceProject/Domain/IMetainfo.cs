using System.Text.Json;

namespace BankingServiceProject.Domain;

public interface IMetainfo
{
    string Serialize(JsonSerializerOptions options)
    {
        return JsonSerializer.Serialize(this, GetType(), options);
    }
}