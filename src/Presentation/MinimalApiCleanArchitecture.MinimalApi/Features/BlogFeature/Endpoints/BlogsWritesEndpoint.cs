using MinimalApiCleanArchitecture.Application.Common.Error.Response;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.AddContributor;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.CreateBlog;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.RemoveContributor;
using MinimalApiCleanArchitecture.MinimalApi.Abstractions;
using MinimalApiCleanArchitecture.MinimalApi.Filters;
using IResult = Microsoft.AspNetCore.Http.IResult;


namespace MinimalApiCleanArchitecture.MinimalApi.Features.BlogFeature.Endpoints
{
    public class BlogsWritesEndpoint : IEndpoint
    {
        private readonly IMediator _mediator;

        public BlogsWritesEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IEndpointRouteBuilder RegisterRoute(IEndpointRouteBuilder endpoints)
        {
            var blogGroup = endpoints.MapGroup("/api/blogs").AddEndpointFilter<ApiExceptionFilter>();

            blogGroup.MapPost("/", CreateBlog)
                .AddEndpointFilter<GuidValidationFilter>()
                //.AddEndpointFilter<ApiExceptionFilter>()
                .WithName("CreateBlog")
                .WithDisplayName("Blog Writes Endpoints")
                .WithTags("Blogs")
                .Produces<SuccessDataResult<CreateBlogResponse>>(201)
                .Produces<ErrorResponse>(500);

            blogGroup.MapPost("/{blogId}/contributors/{contributorId}",AddContributor)
                .AddEndpointFilter<GuidValidationFilter>()
                .WithName("Contributor")
                .WithDisplayName("Blog Writes Endpoints")
                .WithTags("Blogs")
                .Produces<SuccessResult>(200)
                .Produces<ErrorResponse>(404)
                .Produces<ErrorResponse>(500);

            blogGroup.MapDelete("/{id}/contributors/{contributorId}", RemoveContributor)
                .AddEndpointFilter<GuidValidationFilter>()
                .WithName("RemoveContributor")
                .WithDisplayName("Blog Writes Endpoints")
                .WithTags("Blogs")
                .Produces<SuccessResult>(200)
                .Produces<ErrorResponse>(404)
                .Produces<ErrorResponse>(500);

            return blogGroup;
        }

        private async Task<IResult> CreateBlog(CreateBlogRequest blog)
        {
            var command = new CreateBlogCommand (blog.Name,blog.Description,blog.AuthorId);
            var result = await _mediator.Send(command);
            return TypedResults.Ok(result);
        }

        private async Task<IResult> AddContributor(string blogId, string contributorId)
        {
            var result = await _mediator.Send(new AddContributorCommand(Guid.Parse(blogId),Guid.Parse(contributorId)));
            return TypedResults.Ok(result);
        }

        private async Task<IResult> RemoveContributor(string id, string contributorId)
        {
            var result = await _mediator.Send(new RemoveContributorCommand(Guid.Parse(id), Guid.Parse(contributorId)));
            return TypedResults.Ok(result);
        }
    }
}
