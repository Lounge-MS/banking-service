namespace BankingServiceProject.PresentationLayerProject.Webhooks.Rest;

public static class RestWebhookDiExtensions
{
    public static void UseRestWebhookMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<BankingServiceMiddleware>();
    }
}