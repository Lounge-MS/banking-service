using BankingServiceProject.RepositoryProject.Domain;

namespace BankingServiceProject.Domain;

public record PaymentCompletionMessage(
    string PaymentId,
    OperationStatus Status);