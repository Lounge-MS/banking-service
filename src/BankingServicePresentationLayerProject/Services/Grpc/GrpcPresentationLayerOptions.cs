namespace BankingServiceProject.PresentationLayerProject.Grpc;

public record GrpcPresentationLayerOptions
{
    public string OrderServiceIdentityTokenName { get; set; } =
        "ORDER_SERVICE_IDENTITY_TOKEN";
}