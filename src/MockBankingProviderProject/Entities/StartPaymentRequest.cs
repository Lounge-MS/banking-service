namespace BankingServiceProject.MockBankingProviderProject.Entities;

public record StartPaymentRequest(
    decimal Amount,
    string ConfirmationUrl,
    string IdentityToken);