using BankingServiceProject.MockBankingProviderProject.Domain.Payments;

namespace BankingServiceProject.MockBankingProviderProject.Domain;

public record FinishedRollbackWebhookRequest(
    string Id,
    FinishedRollbackStatus Status);