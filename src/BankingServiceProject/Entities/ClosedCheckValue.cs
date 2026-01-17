namespace BankingServiceProject.Entities;

public record ClosedCheckValue(
    string PaymentId,
    ClosedCheckStatus Status);