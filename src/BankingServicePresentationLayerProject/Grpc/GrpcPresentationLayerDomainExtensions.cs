using BankingServiceProject.PresentationLayerProject.Grpc.Exceptions;
using BankingServiceRepositoryProject = BankingServiceProject.RepositoryProject;
using GrpcBankingServiceProject = BankingService;

namespace BankingServiceProject.PresentationLayerProject.Grpc;

public static class GrpcPresentationLayerDomainExtensions
{
    public static BankingServiceRepositoryProject.Domain.BankingProviderType ToDomainEntity(
        this GrpcBankingServiceProject.BankingProviderType providerType)
    {
        return providerType switch
        {
            GrpcBankingServiceProject.BankingProviderType.BankingProviderMock =>
                BankingServiceRepositoryProject.Domain.BankingProviderType.Mock,
            GrpcBankingServiceProject.BankingProviderType.BankingProviderUnspecified or _ =>
                throw new UnknownProviderException(providerType.ToString()),
        };
    }

    public static GrpcBankingServiceProject.BankingProviderType ToGrpcEntity(
        this BankingServiceRepositoryProject.Domain.BankingProviderType providerType)
    {
        return providerType switch
        {
            BankingServiceRepositoryProject.Domain.BankingProviderType.Mock =>
                GrpcBankingServiceProject.BankingProviderType.BankingProviderMock,
            _ => throw new UnknownProviderException(providerType.ToString()),
        };
    }

    public static GrpcBankingServiceProject.PaymentStatus ToGrpc(
        this BankingServiceRepositoryProject.Domain.OperationStatus status)
    {
        return status switch
        {
            BankingServiceRepositoryProject.Domain.OperationStatus.Created =>
                GrpcBankingServiceProject.PaymentStatus.Created,
            BankingServiceRepositoryProject.Domain.OperationStatus.Completed =>
                GrpcBankingServiceProject.PaymentStatus.Completed,
            BankingServiceRepositoryProject.Domain.OperationStatus.Cancelled =>
                GrpcBankingServiceProject.PaymentStatus.Cancelled,
            BankingServiceRepositoryProject.Domain.OperationStatus.Compensated =>
                GrpcBankingServiceProject.PaymentStatus.Compensated,
            _ => GrpcBankingServiceProject.PaymentStatus.Unspecified,
        };
    }

    public static GrpcBankingServiceProject.Payment ToGrpcEntity(
        this BankingServiceRepositoryProject.Domain.OperationEntity entity)
    {
        return new GrpcBankingServiceProject.Payment
        {
            Id = entity.Id,
            AmountInKopecks = (int)(entity.Amount * 100m),
            PaymentUrl = entity.PaymentUrl.AbsoluteUri,
            Status = entity.Status.ToGrpc(),
            Provider = entity.BankingProviderType.ToGrpcEntity(),
        };
    }
}