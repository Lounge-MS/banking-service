using BankingServiceProject.MockBankingProviderProject.Domain.Payments;

namespace BankingServiceProject.MockBankingProviderProject.Domain;

public record FinishedPaymentWebhookRequest(
    string Id,
    FinishedPaymentStatus Status);