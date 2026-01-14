using BankingServiceProject.ExternalConnectorProject;
using BankingServiceProject.SharedProject.Security.Secrets;
using DotNetEnv;

WebApplicationBuilder builder = WebApplication.CreateBuilder();
Env.Load("dev.env");
builder.Configuration.AddJsonFile("appsettings.json");

builder.Services
    .AddEnvironmentSecretsProvider()
    .AddMockStrategy(builder.Configuration.GetSection("BankingProviderStrategies:Mock"))
    .AddExternalConnectorServices(builder.Configuration.GetSection("ExternalConnector"))
    .AddControllers();

builder.Services
    .AddExternalConnectorKafkaProducer(
        builder.Configuration.GetSection("Kafka"),
        builder.Configuration.GetSection("Kafka:Producer:Message"));

WebApplication app = builder.Build();
app.UseExternalConnectorMiddleware();
app.MapControllers();

app.Run();