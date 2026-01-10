namespace BankingServiceProject.Clients.Dto;

public record MockBankingProviderMetainfo(
    byte[] IdentityTokenEncrypted,
    byte[] IdentityTokenIv);