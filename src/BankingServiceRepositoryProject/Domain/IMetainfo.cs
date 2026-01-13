using System.Text.Json;

namespace BankingServiceProject.RepositoryProject.Domain;

public interface IMetainfo
{
    string Serialize(JsonSerializerOptions options)
    {
        return JsonSerializer.Serialize(this, options);
    }
}