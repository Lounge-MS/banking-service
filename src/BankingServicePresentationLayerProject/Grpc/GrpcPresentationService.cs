using Grpc.Core;
using BankingServiceRepositoryProject = BankingServiceProject.RepositoryProject.Domain;
using GrpcBankingServiceProject = BankingService;

namespace BankingServiceProject.PresentationLayerProject.Grpc;

public class GrpcPresentationService : GrpcBankingServiceProject.BankingService.BankingServiceBase
{
    private readonly BankingService _service;

    public GrpcPresentationService(
        BankingService service)
    {
        _service = service;
    }

    public override async Task<GrpcBankingServiceProject.GetPaymentResponse> GetPayment(
        GrpcBankingServiceProject.GetPaymentRequest request,
        ServerCallContext context)
    {
        BankingServiceRepositoryProject.OperationEntity result =
            await _service.GetPaymentAsync(request.Id, context.CancellationToken);

        return new GrpcBankingServiceProject.GetPaymentResponse
        {
            Payment = result.ToGrpcEntity(),
        };
    }

    public override async Task<GrpcBankingServiceProject.CreatePaymentResponse> CreatePayment(
        GrpcBankingServiceProject.CreatePaymentRequest request,
        ServerCallContext context)
    {
        BankingServiceRepositoryProject.OperationEntity result = await _service.CreatePaymentAsync(
            request.IdempotencyKey,
            request.AmountInKopecks / 100m,
            request.Provider.ToDomainEntity(),
            context.CancellationToken);

        return new GrpcBankingServiceProject.CreatePaymentResponse
        {
            Payment = result.ToGrpcEntity(),
        };
    }

    public override async Task<GrpcBankingServiceProject.MarkCompensatedResponse> MarkCompensated(
        GrpcBankingServiceProject.MarkCompensatedRequest request,
        ServerCallContext context)
    {
        await _service.MarkCompensatedAsync(request.PaymentId, context.CancellationToken);

        return new GrpcBankingServiceProject.MarkCompensatedResponse();
    }
}