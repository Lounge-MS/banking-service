namespace BankingServiceProject.Clients.Dto;

public record MockBankingProviderStartPaymentRequest(
    decimal Amount,
    Uri ConfirmationUrl,
    string IdentityToken);