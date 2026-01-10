using BankingServiceProject.RepositoryProject.Converters;
using System.Text.Json.Serialization;

namespace BankingServiceProject.RepositoryProject.Domain;

[JsonConverter(typeof(UppercaseJsonEnumConverter<BankingProvider>))]
public enum BankingProvider
{
    Mock,
}