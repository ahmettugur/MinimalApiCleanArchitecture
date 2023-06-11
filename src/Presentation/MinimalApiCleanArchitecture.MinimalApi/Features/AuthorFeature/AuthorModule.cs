using MinimalApiCleanArchitecture.Application.Interfaces.GrpcServices.AuthorGrpc;
using MinimalApiCleanArchitecture.MinimalApi.Abstractions;
using MinimalApiCleanArchitecture.MinimalApi.Features.AuthorFeature.Endpoints;

namespace MinimalApiCleanArchitecture.MinimalApi.Features.AuthorFeature;

public class AuthorModule : IModule
{
    private IAuthorGrpcService? _authorGrpcService;
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        new AuthorReadsEndpoint(_authorGrpcService!).RegisterRoute(endpoints);
        new AuthorWritesEndpoint( _authorGrpcService!).RegisterRoute(endpoints);
        return endpoints;
    }

    public WebApplicationBuilder RegisterModule(WebApplicationBuilder builder)
    {
        var provider = builder.Services.BuildServiceProvider();
        _authorGrpcService = provider.GetRequiredService<IAuthorGrpcService>();
        return builder;
    }
}