using BankingServiceProject;
using BankingServiceProject.CommonProject.Security.Cryptography;
using BankingServiceProject.CommonProject.Security.Secrets;
using BankingServiceProject.PresentationLayerProject.Grpc;
using BankingServiceProject.RepositoryProject;
using DotNetEnv;

WebApplicationBuilder builder = WebApplication.CreateBuilder();

Env.Load(".env");
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();

var secretsProvider = new EnvironmentSecretsProvider();

builder.Services
    .AddSecretsProvider(secretsProvider)
    .AddAesEncryptor();

string connectionString = secretsProvider.GetSecretString("POSTGRES_CONNECTION_STRING");

builder.Services
    .AddBankingServiceRepositoryMigrations(connectionString)
    .AddBankingServiceKafkaProducer(
        builder.Configuration.GetSection("Kafka"),
        builder.Configuration.GetSection("Kafka:Producer:Message"))
    .AddMockBankingProviderStrategy(builder.Configuration.GetSection("BankingProviders:Mock"))
    .AddBankingServiceServices(
        connectionString,
        builder.Configuration.GetSection("BankingService"))
    .AddGrpcPresentationLayerServices(
        builder.Configuration.GetSection("Presentation:Grpc"))
    .AddControllers();

WebApplication app = builder.Build();

app.Services.RunBankingServiceRepositoryMigrations();
app.MapGrpcPresentationLayer();
app.MapControllers();

await app.RunAsync();