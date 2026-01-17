using BankingServiceProject.Entities.Dto;

namespace BankingServiceProject.Strategies.Mock.Dto;

public record MockBankingProviderMetainfo(
    string Id,
    byte[] IdentityTokenEncrypted,
    byte[] IdentityTokenIv)
    : IMetainfo;