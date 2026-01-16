namespace BankingServiceProject.Entities.Dto;

public record PaymentCompletionMessage(
    string PaymentId,
    OperationStatus Status);