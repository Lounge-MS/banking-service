namespace BankingServiceProject.PresentationLayerProject.Services.Grpc.Exceptions;

public class GrpcPresentationLayerException : Exception
{
    protected GrpcPresentationLayerException(
        string message = "Error occured in the gRPC presentation layer")
        : base(message)
    {
    }
}