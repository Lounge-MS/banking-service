using BankingServiceProject.MockBankingProviderProject;
using System.Text.Json;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder();
IConfigurationSection configuration = builder.Configuration.GetSection("Configuration");

builder.Services.AddSingleton(new JsonSerializerOptions
{
    Converters = { new JsonStringEnumConverter() },
});

builder.Services
    .AddHttpClient()
    .AddControllers();

builder.Services.Configure<MockBankingOptions>(configuration);
builder.Services.AddSingleton<MockBankingProvider>();

WebApplication app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();
await app.RunAsync();