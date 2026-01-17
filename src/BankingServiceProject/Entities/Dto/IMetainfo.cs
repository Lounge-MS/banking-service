using System.Text.Json;

namespace BankingServiceProject.Entities.Dto;

public interface IMetainfo
{
    string Serialize(JsonSerializerOptions options)
    {
        return JsonSerializer.Serialize(this, GetType(), options);
    }
}