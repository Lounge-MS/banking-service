namespace BankingServiceProject.PresentationLayerProject.Services.Grpc.Exceptions;

public class UnknownProviderException
    : GrpcPresentationLayerException
{
    public UnknownProviderException(
        string providerName)
        : base($"Received unknown banking provider (\"{providerName}\")")
    {
    }
}