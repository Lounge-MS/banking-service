WebApplicationBuilder builder = WebApplication.CreateBuilder();
WebApplication app = builder.Build();
await app.RunAsync();