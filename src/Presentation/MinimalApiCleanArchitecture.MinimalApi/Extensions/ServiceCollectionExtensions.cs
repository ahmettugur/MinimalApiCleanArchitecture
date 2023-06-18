using Common.OpenTelemetry.Extensions;
using MinimalApiCleanArchitecture.Application;
using MinimalApiCleanArchitecture.Infrastructure;
using MinimalApiCleanArchitecture.Persistence;

namespace MinimalApiCleanArchitecture.MinimalApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = builder.Environment.ApplicationName, Version = "v1" });
            c.TagActionsBy(ta => new List<string> { ta.ActionDescriptor.DisplayName!});
        });

        builder.Services.AddApplicationServices();
        builder.Services.AddPersistenceServices(builder.Configuration);
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddOpenTelemetryServices(builder.Configuration);
        builder.Services.ConfigureConsul(builder.Configuration);

        return builder.Services;
    }
}