using BankingServiceProject.RepositoryProject.Converters;
using System.Text.Json.Serialization;

namespace BankingServiceProject.RepositoryProject.Domain;

[JsonConverter(typeof(UppercaseJsonEnumConverter<OperationStatus>))]
public enum OperationStatus
{
    Started,
    Completed,
    Cancelled,
    Compensating,
    Compensated,
}