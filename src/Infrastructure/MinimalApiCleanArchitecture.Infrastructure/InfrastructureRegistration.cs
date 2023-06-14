using System.Reflection;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.Application.Interfaces.GrpcServices.AuthorGrpc;
using MinimalApiCleanArchitecture.Infrastructure.Services.GrpcServices.AuthorGrpc;
using MinimalApiCleanArchitecture.Infrastructure.Protos;

namespace MinimalApiCleanArchitecture.Infrastructure;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
    {
        
        services.AddGrpcClient<AuthorProtoService.AuthorProtoServiceClient>(o =>
        {
            var grpcServiceAddress = configuration["GrpcSettings:AuthorGrpcServiceUrl"]!;
            var serviceName = configuration["GrpcSettings:AuthorGrpcServiceConsulName"]!;
            var consulClient = services.BuildServiceProvider().GetService<IConsulClient>();
            var allRegisteredServices = consulClient?.Agent.Services().GetAwaiter().GetResult();

            var registeredServices = allRegisteredServices?.Response?.Where(s => s.Key.Equals(serviceName, StringComparison.OrdinalIgnoreCase)).Select(x => x.Value).ToList();
            if (registeredServices is {Count: > 0})
            {
                var consulService = registeredServices[0];
                var uriBuilder = new UriBuilder()
                {
                    Host = consulService.Address,
                    Port = consulService.Port
                };
                grpcServiceAddress = uriBuilder.Uri.ToString().TrimEnd('/');
            }

            o.Address = new Uri(grpcServiceAddress);

        });
        services.AddScoped<IAuthorGrpcService, AuthorGrpcService>();

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }

}