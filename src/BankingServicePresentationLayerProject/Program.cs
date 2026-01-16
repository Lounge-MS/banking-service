using BankingServiceProject;
using BankingServiceProject.CommonProject.Security.Cryptography;
using BankingServiceProject.CommonProject.Security.Secrets;
using BankingServiceProject.PresentationLayerProject.Brokers.Kafka;
using BankingServiceProject.PresentationLayerProject.Services.Grpc;
using BankingServiceProject.PresentationLayerProject.Webhooks.Rest;
using BankingServiceProject.RepositoryProject;
using DotNetEnv;

WebApplicationBuilder builder = WebApplication.CreateBuilder();

Env.Load("example.env");
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();

var secretsProvider = new EnvironmentSecretsProvider();

builder.Services
    .AddSecretsProvider(secretsProvider)
    .AddAesEncryptor();

string connectionString = secretsProvider.GetSecretString("POSTGRES_CONNECTION_STRING");

builder.Services.AddBankingServiceServices(
    builder.Configuration.GetSection("BankingService"));

builder.Services
    .AddBankingServicePostgresMigrations(connectionString)
    .AddBankingServicePostgresRepository(connectionString);

builder.Services
    .AddMockBankingProviderStrategy(
        builder.Configuration.GetSection("BankingProviders:Mock"))
    .AddRefitStrategyMockClient();

builder.Services
    .AddBankingServiceKafkaProducer(
        builder.Configuration.GetSection("Kafka"),
        builder.Configuration.GetSection("Kafka:Producer:Message"))
    .AddGrpcPresentationLayerServices(
        builder.Configuration.GetSection("Presentation:Grpc"))
    .AddControllers();

WebApplication app = builder.Build();

app.UseRestWebhookMiddleware();
app.Services.RunBankingServiceRepositoryMigrations();
app.MapGrpcPresentationLayer();
app.MapControllers();

await app.RunAsync();