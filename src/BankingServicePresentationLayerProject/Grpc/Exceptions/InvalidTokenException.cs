namespace BankingServiceProject.PresentationLayerProject.Grpc.Exceptions;

public class InvalidTokenException : GrpcPresentationLayerException
{
    public InvalidTokenException() : base("Token is invalid")
    {
    }
}