
using System.Reflection;
using Common.Logging;
using MinimalApiCleanArchitecture.GrpcService.Extensions;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = SeriLogger.CustomLoggerConfiguration(builder.Configuration);
builder.Host.UseSerilog();

builder.Services.AddServices(builder);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.


var app = builder.Build();
app.ConfigureApplication();
app.Run();