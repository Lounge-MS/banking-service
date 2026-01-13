using BankingServiceProject.Domain;

namespace BankingServiceProject.ProviderStrategies.Mock.Dto;

public record MockBankingProviderMetainfo(
    string Id,
    byte[] IdentityTokenEncrypted,
    byte[] IdentityTokenIv)
    : IMetainfo;