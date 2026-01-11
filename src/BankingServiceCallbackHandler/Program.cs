#pragma warning disable CA1506
using BankingServiceCallbackHandler;
using BankingServiceProject.SharedProject.Cryptography;
using Confluent.Kafka;
using DotNetEnv;
using Itmo.Dev.Platform.Kafka.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder();
Env.Load("dev.env");
builder.Configuration.AddJsonFile("appsettings.json");

builder.Services
    .AddSingleton<ISecretsProvider, EnvironmentSecretsProvider>()
    .AddSingleton<CallbackHandlerService>()
    .AddSingleton<BankingProviderValidator>()
    .AddControllers();

builder.Services
    .Configure<CallbackHandlerOptions>(builder.Configuration.GetSection("CallbackHandler"));

builder.Services.AddPlatformKafka(
    selector => selector
        .ConfigureOptions(builder.Configuration.GetSection("Kafka"))
        .AddProducer(b => b
            .WithKey<Null?>()
            .WithValue<object>()
            .WithConfiguration(builder.Configuration.GetSection("Kafka:Producers:Message"))
            .SerializeKeyWithNewtonsoft()
            .SerializeValueWithNewtonsoft()));

builder.Services.AddHostedService(
    provider => provider.GetRequiredService<CallbackHandlerService>());
WebApplication app = builder.Build();
app.UseMiddleware<CallbackHandlerMiddleware>();
app.MapControllers();

app.Run();