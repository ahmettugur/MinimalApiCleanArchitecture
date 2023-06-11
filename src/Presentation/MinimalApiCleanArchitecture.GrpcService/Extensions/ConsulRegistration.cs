using Consul;

namespace MinimalApiCleanArchitecture.GrpcService.Extensions;

public static class ConsulRegistration
{
    public static IServiceCollection ConfigureConsul(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
        {
            var address = configuration["ConsulConfig:Address"]!;
            consulConfig.Address = new Uri(address);
        }));

        return services;
    }
}