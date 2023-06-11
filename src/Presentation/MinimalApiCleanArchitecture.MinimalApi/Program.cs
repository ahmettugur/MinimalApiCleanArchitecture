using Common.Logging;
using MinimalApiCleanArchitecture.MinimalApi.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
Log.Logger = SeriLogger.CustomLoggerConfiguration(builder.Configuration);

builder.Services.AddServices(builder);
builder.RegisterModules();

var app = builder.Build();
app.ConfigureApplication();
app.MapEndpoints();

app.Run();
