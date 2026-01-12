using BankingServiceCallbackHandlerProject;
using BankingServiceCallbackHandlerProject.ProviderStrategies;
using DotNetEnv;

WebApplicationBuilder builder = WebApplication.CreateBuilder();
Env.Load("dev.env");
builder.Configuration.AddJsonFile("appsettings.json");

builder.Services
    .Configure<MockBankingProviderStrategyOptions>(
        builder.Configuration.GetSection("BankingProviderStrategies:Mock"))
    .Configure<CallbackHandlerOptions>(
        builder.Configuration.GetSection("CallbackHandler"));

builder.Services
    .AddCallbackHandlerRequiredServices()
    .AddControllers();

builder.Services
    .AddCallbackHandlerKafkaProducer(
        builder.Configuration.GetSection("Kafka"),
        builder.Configuration.GetSection("Kafka:Producer:Message"));

WebApplication app = builder.Build();
app.UseCallbackHandlerMiddleware();
app.MapControllers();

app.Run();