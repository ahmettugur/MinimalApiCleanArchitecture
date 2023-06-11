using MinimalApiCleanArchitecture.Application.Common.Error.Response;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetAllBlogs;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetBlogById;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetBlogContributors;
using MinimalApiCleanArchitecture.MinimalApi.Abstractions;
using MinimalApiCleanArchitecture.MinimalApi.Filters;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace MinimalApiCleanArchitecture.MinimalApi.Features.BlogFeature.Endpoints
{
    public class BlogReadsEndpoint : IEndpoint
    {
        private readonly IMediator _mediator;

        public BlogReadsEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IEndpointRouteBuilder RegisterRoute(IEndpointRouteBuilder endpoints)
        {
            var blogGroup = endpoints.MapGroup("/api/blogs").AddEndpointFilter<ApiExceptionFilter>();
            
            blogGroup.MapGet("/", GetAllBlogs)
                .WithName("GetAllBlogs")
                .WithDisplayName("Blog Reads Endpoints")
                .WithTags("Blogs")
                .Produces<SuccessDataResult<List<GetAllBlogsResponse>>>()
                .Produces(500);
            
            blogGroup.MapGet("/{id}", GetBlogById)
                .AddEndpointFilter<GuidValidationFilter>()
                .WithName("GetBlogById")
                .WithDisplayName("Blog Reads Endpoints")
                .WithTags("Blogs")
                .Produces<SuccessDataResult<GetBlogByIdResponse>>(200)
                .Produces<ErrorResponse>(404)
                .Produces<ErrorResponse>(500);
            
            blogGroup.MapGet("/{id}/contributors", GetBlogContributors)
                .AddEndpointFilter<GuidValidationFilter>()
                .WithName("GetBlogContributors")
                .WithDisplayName("Blog Reads Endpoints")
                .WithTags("Blogs")
                .Produces<SuccessDataResult<List<GetBlogContributorsResponse>>>(200)
                .Produces(500)
                .Produces<ErrorResponse>(404)
                .Produces<ErrorResponse>(500);
            
            return blogGroup;
        }

        private async Task<IResult> GetBlogById(string id)
        {
            var blog = await _mediator.Send(new GetBlogByIdQuery(Guid.Parse(id)));
            return TypedResults.Ok(blog);
        }
        private async Task<IResult> GetAllBlogs()
        {
            var blogs = await _mediator.Send(new GetAllBlogsQuery());
            return TypedResults.Ok(blogs);
        }

        private async Task<IResult> GetBlogContributors(string id)
        {
            var result = await _mediator.Send(new GetBlogContributorsQuery(Guid.Parse(id)));
            return TypedResults.Ok(result);
        }
    }
}