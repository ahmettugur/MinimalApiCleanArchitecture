using Common.OpenTelemetry.Middlewares;
using Consul;
using MinimalApiCleanArchitecture.Persistence;

namespace MinimalApiCleanArchitecture.MinimalApi.Extensions;

public static class WebApplicationExtensions
{
  public static WebApplication ConfigureApplication(this WebApplication app)
  {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
    app.UseMiddleware<OpenTelemetryTraceIdMiddleware>();
    app.UseMiddleware<RequestAndResponseActivityMiddleware>();
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
      ID = serviceId ?? "MinimalApiCleanArchitectureMinimalApi",
      Name = serviceName ?? "MinimalApiCleanArchitectureMinimalApi",
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