using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.ApplicationDefaultConnection.IntegrationTests;
using MinimalApiCleanArchitecture.Persistence;
namespace MinimalApiCleanArchitecture.Application.IntegrationTests;

using static Testing;

internal class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            var integrationConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            configurationBuilder.AddConfiguration(integrationConfig);
        });

        builder.ConfigureServices((builder, services) =>
        {
            services
                .Remove<DbContextOptions<MinimalApiCleanArchitectureDbContext>>()
                .AddDbContext<MinimalApiCleanArchitectureDbContext>((sp, options) =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"),
                        builder => builder.MigrationsAssembly(typeof(MinimalApiCleanArchitectureDbContext).Assembly.FullName)));
        });
    }
}