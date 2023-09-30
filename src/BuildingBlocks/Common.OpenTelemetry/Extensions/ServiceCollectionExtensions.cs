using System.Diagnostics;
using Common.OpenTelemetry.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;


namespace Common.OpenTelemetry.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenTelemetryServices(this IServiceCollection services,IConfiguration configuration)
    {
        var jaegerSettings = configuration.GetSection(nameof(JaegerSettings))
            .Get<JaegerSettings>()!;
        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .AddSource(jaegerSettings.SourceName)
                    .ConfigureResource(resource => resource
                        .AddService(jaegerSettings.ServiceName))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddGrpcCoreInstrumentation()
                    //.AddSqlClientInstrumentation()
                    .AddSqlClientInstrumentation(opt =>
                    {
                        opt.SetDbStatementForText = true;
                        opt.EnableConnectionLevelAttributes = true;
                        opt.SetDbStatementForStoredProcedure = true;
                    });

                var exporter = configuration["ExporterSettings:UseExporter"]!.ToLowerInvariant();
                switch (exporter)
                {
                    case "jaeger":
                        tracerProviderBuilder.AddOtlpExporter(jaegerOptions =>
                        {
                            jaegerOptions.Endpoint = new Uri($"{jaegerSettings.Host}:{jaegerSettings.Port}");
                        });
                        break;
                    case "zipkin":
                        tracerProviderBuilder.AddZipkinExporter(zipkinOptions =>
                        {
                            zipkinOptions.Endpoint = new Uri(configuration.GetValue<string>("Zipkin:Endpoint")!);
                        });
                        break;
                    default:
                        tracerProviderBuilder.AddConsoleExporter();
                        break;
                }
            });
        
        return services;
    }
}
