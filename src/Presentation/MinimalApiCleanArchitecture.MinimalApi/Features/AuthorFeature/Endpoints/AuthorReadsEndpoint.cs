using MinimalApiCleanArchitecture.Application.Common.Error.Response;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAllAuthors;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAuthorById;
using MinimalApiCleanArchitecture.Application.Interfaces.GrpcServices.AuthorGrpc;
using MinimalApiCleanArchitecture.MinimalApi.Abstractions;
using MinimalApiCleanArchitecture.MinimalApi.Filters;

namespace MinimalApiCleanArchitecture.MinimalApi.Features.AuthorFeature.Endpoints
{
    public class AuthorReadsEndpoint : IEndpoint
    {
        private readonly IAuthorGrpcService _authorGrpcService;

        public AuthorReadsEndpoint(IAuthorGrpcService authorGrpcService)
        {
            _authorGrpcService = authorGrpcService;
        }

        public IEndpointRouteBuilder RegisterRoute(IEndpointRouteBuilder endpoints)
        {
            var authorGroup = endpoints.MapGroup("/api/authors").AddEndpointFilter<ApiExceptionFilter>();
            
            authorGroup.MapGet("/",GetAllAuthors)
                .WithName("GetAllAuthors")
                .WithDisplayName("Author Reads Endpoints")
                .WithTags("Authors")
                .Produces<List<GetAllAuthorsResponse>>()
                .Produces(500);
            
            authorGroup.MapGet("/{id}", GetAuthorById)
                .AddEndpointFilter<GuidValidationFilter>()
                .WithName("GetAuthorById")
                .WithDisplayName("Author Reads Endpoints")
                .WithTags("Authors")
                .Produces<GetAuthorByIdResponse>(200)
                .Produces<ErrorResponse>(404)
                .Produces<ErrorResponse>(500);


            return authorGroup;

        }
        
        private async Task<IResult> GetAllAuthors()
        {
            var authors = await _authorGrpcService.GetAuthorsAsync();
            return Results.Ok(authors);
        }


        private async Task<IResult> GetAuthorById(string id)
        {
            var author = await _authorGrpcService.GetAuthorByIdAsync(id);
            return TypedResults.Ok(author);
        }

    }
}
