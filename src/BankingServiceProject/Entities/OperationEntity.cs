using System.Text.Json;

namespace BankingServiceProject.Entities;

public record OperationEntity(
    string Id,
    string IdempotencyKey,
    JsonDocument Metainfo,
    Uri PaymentUrl,
    decimal Amount,
    OperationStatus Status,
    BankingProviderType BankingProviderType,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public BrokerMessage<ClosedCheckKey, ClosedCheckValue> ToBrokerMessage()
    {
        ClosedCheckStatus status = Status switch
        {
            OperationStatus.Completed => ClosedCheckStatus.Completed,
            OperationStatus.Cancelled => ClosedCheckStatus.Cancelled,
            OperationStatus.Created or OperationStatus.Compensated or _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(Status),
                    "Impossible state in current context"),
        };

        var key = new ClosedCheckKey(Id);
        var value = new ClosedCheckValue(Id, status);
        return new BrokerMessage<ClosedCheckKey, ClosedCheckValue>(key, value);
    }
}