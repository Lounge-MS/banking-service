using BankingServiceProject.Entities;
using BankingServiceProject.PresentationLayerProject.Services.Grpc.Exceptions;
using GrpcBankingService;
using BankingProviderType = GrpcBankingService.BankingProviderType;

namespace BankingServiceProject.PresentationLayerProject.Services.Grpc;

public static class GrpcPresentationLayerDomainExtensions
{
    public static Entities.BankingProviderType ToDomainEntity(
        this BankingProviderType providerType)
    {
        return providerType switch
        {
            BankingProviderType.BankingProviderMock =>
                Entities.BankingProviderType.Mock,
            BankingProviderType.BankingProviderUnspecified or _ =>
                throw new UnknownProviderException(providerType.ToString()),
        };
    }

    public static BankingProviderType ToGrpcEntity(
        this Entities.BankingProviderType providerType)
    {
        return providerType switch
        {
            Entities.BankingProviderType.Mock =>
                BankingProviderType.BankingProviderMock,
            _ => throw new UnknownProviderException(providerType.ToString()),
        };
    }

    public static GrpcBankingService.PaymentStatus ToGrpc(
        this Entities.OperationStatus status)
    {
        return status switch
        {
            OperationStatus.Created =>
                PaymentStatus.Created,
            OperationStatus.Completed =>
                PaymentStatus.Completed,
            OperationStatus.Cancelled =>
                PaymentStatus.Cancelled,
            OperationStatus.Compensated =>
                PaymentStatus.Compensated,
            _ => PaymentStatus.Unspecified,
        };
    }

    public static Payment ToGrpcEntity(
        this OperationEntity entity)
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