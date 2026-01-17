namespace BankingServiceProject.Strategies.Mock.Dto;

public record MockBankingProviderStartPaymentRequest(
    decimal Amount,
    Uri ConfirmationUrl,
    string IdentityToken);