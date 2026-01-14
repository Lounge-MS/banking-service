using System.Text.Json;

namespace BankingServiceProject.ExternalConnectorProject.Domain;

public interface IMetainfo
{
    string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }
}