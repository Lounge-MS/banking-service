using BankingServiceProject;
using BankingServiceProject.CommonProject.Security.Cryptography;
using BankingServiceProject.CommonProject.Security.Secrets;
using BankingServiceProject.RepositoryProject;
using BankingServiceProject.RepositoryProject.Domain;
using BankingServiceProject.RepositoryProject.Repositories;
using DotNetEnv;

WebApplicationBuilder builder = WebApplication.CreateBuilder();

Env.Load("dev.env");
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
    .AddControllers();

WebApplication app = builder.Build();
app.Services.RunBankingServiceRepositoryMigrations();
app.MapControllers();

OperationsRepository repository = app.Services.GetRequiredService<OperationsRepository>();
BankingService service = app.Services.GetRequiredService<BankingService>();
string guid = Guid.NewGuid().ToString();

Console.WriteLine(
    await service.StartPaymentAsync(guid, 50, BankingProviderType.Mock, CancellationToken.None));

Console.WriteLine(
    await repository.GetOperationByIdempotencyKeyAsync(guid));

await app.RunAsync();