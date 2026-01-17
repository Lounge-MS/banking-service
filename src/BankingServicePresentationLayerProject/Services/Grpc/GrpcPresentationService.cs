using BankingServiceProject.Entities;
using BankingServiceProject.Ports.Services;
using Grpc.Core;
using GrpcBankingServiceProject = GrpcBankingService;

namespace BankingServiceProject.PresentationLayerProject.Services.Grpc;

public class GrpcPresentationService : GrpcBankingServiceProject.BankingService.BankingServiceBase
{
    private readonly IBankingService _service;

    public GrpcPresentationService(
        IBankingService service)
    {
        _service = service;
    }

    public override async Task<GrpcBankingServiceProject.GetPaymentResponse> GetPayment(
        GrpcBankingServiceProject.GetPaymentRequest request,
        ServerCallContext context)
    {
        OperationEntity result =
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
        OperationEntity result = await _service.CreatePaymentAsync(
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
        await _service.MarkCompensatedAsync(request.Id, context.CancellationToken);

        return new GrpcBankingServiceProject.MarkCompensatedResponse();
    }
}