namespace BankingServiceProject.PresentationLayerProject.Services.Grpc;

public record GrpcPresentationLayerOptions
{
    public string OrderServiceIdentityTokenName { get; set; } =
        "ORDER_SERVICE_IDENTITY_TOKEN";
}