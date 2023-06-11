namespace MinimalApiCleanArchitecture.MinimalApi.Abstractions
{
    public interface IEndpoint
    {
        IEndpointRouteBuilder RegisterRoute(IEndpointRouteBuilder endpoints);
    }
}
