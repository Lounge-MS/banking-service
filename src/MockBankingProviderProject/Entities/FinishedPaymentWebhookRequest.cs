using BankingServiceProject.MockBankingProviderProject.Entities.Payments;

namespace BankingServiceProject.MockBankingProviderProject.Entities;

public record FinishedPaymentWebhookRequest(
    string Id,
    FinishedPaymentStatus Status);