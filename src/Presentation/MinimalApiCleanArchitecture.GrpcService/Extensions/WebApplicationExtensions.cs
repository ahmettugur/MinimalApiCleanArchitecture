using Common.OpenTelemetry.Middlewares;
using Consul;
using Microsoft.EntityFrameworkCore;
using MinimalApiCleanArchitecture.GrpcService.Services;
using MinimalApiCleanArchitecture.Persistence;

namespace MinimalApiCleanArchitecture.GrpcService.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        app.UseMiddleware<OpenTelemetryTraceIdMiddleware>();
        app.MapGrpcService<AuthorService>();
        app.MapGet("/",
            () =>
                "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        
        //Db Migration
        MigrateDatabase(app);
        app.RegisterWithConsul();
        return app;
    }
    
    private static void RegisterWithConsul(this WebApplication app)
    {
        var consulClient = app.Services.GetRequiredService<IConsulClient>();
        var uri = app.Configuration.GetValue<Uri>("ConsulConfig:ServiceAddress");
        var serviceName = app.Configuration.GetValue<string>("ConsulConfig:ServiceName");
        var serviceId = app.Configuration.GetValue<string>("ConsulConfig:ServiceId");
    
        var registration = new AgentServiceRegistration()
        {
            ID = serviceId ?? "GrpcService",
            Name = serviceName ?? "GrpcService",
            Address = $"{uri!.Host}",
            Port = uri.Port,
            Tags = new[] { serviceName, serviceId }
        };

        app.Logger.LogInformation("Registering with Consul");
        consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        consulClient.Agent.ServiceRegister(registration).Wait();

        app.Lifetime.ApplicationStopping.Register(() =>
        {
            app.Logger.LogInformation("DeRegistering from Consul");
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        });
    }
    private static void MigrateDatabase(IHost app)
    {
        using var scope = app.Services.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<MinimalApiCleanArchitectureDbContext>();
        dataContext.Database.Migrate();
    }
}