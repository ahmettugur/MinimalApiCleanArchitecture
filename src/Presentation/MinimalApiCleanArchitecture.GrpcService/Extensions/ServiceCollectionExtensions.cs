using System.Reflection;
using Common.OpenTelemetry.Extensions;
using MinimalApiCleanArchitecture.Application;
using MinimalApiCleanArchitecture.Persistence;

namespace MinimalApiCleanArchitecture.GrpcService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        builder.Services.AddApplicationServices();
        builder.Services.AddPersistenceServices(builder.Configuration);
        builder.Services.AddOpenTelemetryServices(builder.Configuration);
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
        builder.Services.ConfigureConsul(builder.Configuration);
        
        builder.Services.AddGrpc();
        
        return builder.Services;
    }
}