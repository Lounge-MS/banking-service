using Grpc.Core;
using Grpc.Core.Interceptors;

namespace BankingServiceProject.PresentationLayerProject.Services.Grpc;

public class GrpcPresentationLayerAuthInterceptor : Interceptor
{
    private readonly TokenValidator _tokenValidator;

    public GrpcPresentationLayerAuthInterceptor(
        TokenValidator tokenValidator)
    {
        _tokenValidator = tokenValidator;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        string? token = context.RequestHeaders
            .FirstOrDefault(h => h.Key == "authorization")
            ?.Value;

        if (!_tokenValidator.ValidateToken(token))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid token"));
        }

        return await continuation(request, context);
    }
}