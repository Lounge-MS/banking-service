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
        catch (RpcException)
        {
            throw;
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
        catch (InvalidStateException ex)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message));
        }
        catch (EntityNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unhandled exception: {ex}");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }
}