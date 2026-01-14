namespace BankingServiceProject.PresentationLayerProject.Grpc;

public static class GrpcPresentationLayerDiExtensions
{
    public static IServiceCollection AddGrpcPresentationLayerServices(
        this IServiceCollection serviceCollection,
        IConfigurationSection configSection)
    {
        serviceCollection.AddGrpc(opts =>
        {
            opts.Interceptors.Add<GrpcPresentationLayerAuthInterceptor>();
            opts.Interceptors.Add<GrpcErrorHandlingInterceptor>();
        });
        serviceCollection.AddSingleton<TokenValidator>();
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