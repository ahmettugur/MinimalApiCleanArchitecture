using MinimalApiCleanArchitecture.MinimalApi.Abstractions;
using MinimalApiCleanArchitecture.MinimalApi.Features.BlogFeature.Endpoints;

namespace MinimalApiCleanArchitecture.MinimalApi.Features.BlogFeature
{
    public class BlogsModule : IModule
    {
        private IMediator? _mediator;
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            new BlogReadsEndpoint(_mediator!).RegisterRoute(endpoints);
            new BlogsWritesEndpoint(_mediator!).RegisterRoute(endpoints);

            return endpoints;
        }

        public WebApplicationBuilder RegisterModule(WebApplicationBuilder builder)
        {
            var provider = builder.Services.BuildServiceProvider();
            _mediator = provider.GetRequiredService<IMediator>();
            return builder;
        }
    }
}
