namespace BankingServiceProject.PresentationLayerProject.Grpc;

public static class GrpcPresentationLayerDiExtensions
{
    public static IServiceCollection AddGrpcPresentationLayerServices(
        this IServiceCollection serviceCollection,
        IConfigurationSection configSection)
    {
        serviceCollection.AddGrpc();
        return serviceCollection
            .AddOptions()
            .Configure<GrpcPresentationLayerOptions>(configSection);
    }

    public static void MapGrpcPresentationLayer(
        this IEndpointRouteBuilder serviceCollection)
    {
        serviceCollection.MapGrpcService<GrpcPresentationService>();
    }
}