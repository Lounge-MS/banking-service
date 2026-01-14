using BankingServiceProject.Exceptions;
using BankingServiceProject.PresentationLayerProject.Grpc.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace BankingServiceProject.PresentationLayerProject.Grpc;

public class GrpcErrorHandlingInterceptor : Interceptor
{
    private readonly ILogger<GrpcErrorHandlingInterceptor> _logger;

    public GrpcErrorHandlingInterceptor(
        ILogger<GrpcErrorHandlingInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (InvalidTokenException ex)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, ex.Message));
        }
        catch (UnknownProviderException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (ChannelFullException ex)
        {
            throw new RpcException(new Status(StatusCode.ResourceExhausted, ex.Message));
        }
        catch (InvalidMessageException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unhandled exception: {ex}");
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
    }
}