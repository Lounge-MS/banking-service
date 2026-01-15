using BankingServiceProject.PresentationLayerProject.Grpc.Exceptions;
using GrpcBankingService;
using BankingServiceRepositoryProject = BankingServiceProject.RepositoryProject;

namespace BankingServiceProject.PresentationLayerProject.Grpc;

public static class GrpcPresentationLayerDomainExtensions
{
    public static BankingServiceRepositoryProject.Domain.BankingProviderType ToDomainEntity(
        this BankingProviderType providerType)
    {
        return providerType switch
        {
            BankingProviderType.BankingProviderMock =>
                BankingServiceRepositoryProject.Domain.BankingProviderType.Mock,
            BankingProviderType.BankingProviderUnspecified or _ =>
                throw new UnknownProviderException(providerType.ToString()),
        };
    }

    public static BankingProviderType ToGrpcEntity(
        this BankingServiceRepositoryProject.Domain.BankingProviderType providerType)
    {
        return providerType switch
        {
            BankingServiceRepositoryProject.Domain.BankingProviderType.Mock =>
                BankingProviderType.BankingProviderMock,
            _ => throw new UnknownProviderException(providerType.ToString()),
        };
    }

    public static GrpcBankingService.PaymentStatus ToGrpc(
        this BankingServiceRepositoryProject.Domain.OperationStatus status)
    {
        return status switch
        {
            BankingServiceRepositoryProject.Domain.OperationStatus.Created =>
                PaymentStatus.Created,
            BankingServiceRepositoryProject.Domain.OperationStatus.Completed =>
                PaymentStatus.Completed,
            BankingServiceRepositoryProject.Domain.OperationStatus.Cancelled =>
                PaymentStatus.Cancelled,
            BankingServiceRepositoryProject.Domain.OperationStatus.Compensated =>
                PaymentStatus.Compensated,
            _ => PaymentStatus.Unspecified,
        };
    }

    public static Payment ToGrpcEntity(
        this BankingServiceRepositoryProject.Domain.OperationEntity entity)
    {
        return new Payment
        {
            Id = entity.Id,
            AmountInKopecks = (int)(entity.Amount * 100m),
            PaymentUrl = entity.PaymentUrl.AbsoluteUri,
            Status = entity.Status.ToGrpc(),
            Provider = entity.BankingProviderType.ToGrpcEntity(),
        };
    }
}