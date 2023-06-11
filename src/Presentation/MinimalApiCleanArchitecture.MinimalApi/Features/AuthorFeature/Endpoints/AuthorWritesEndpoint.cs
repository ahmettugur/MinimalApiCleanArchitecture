using MinimalApiCleanArchitecture.Application.Common.Error.Response;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.DeleteAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.UpdateAuthor;
using MinimalApiCleanArchitecture.Application.Interfaces.GrpcServices.AuthorGrpc;
using MinimalApiCleanArchitecture.MinimalApi.Abstractions;
using MinimalApiCleanArchitecture.MinimalApi.Filters;

namespace MinimalApiCleanArchitecture.MinimalApi.Features.AuthorFeature.Endpoints
{
    public class AuthorWritesEndpoint : IEndpoint
    {

        private readonly IAuthorGrpcService _authorGrpcService;

        public AuthorWritesEndpoint( IAuthorGrpcService authorGrpcService)
        {
            _authorGrpcService = authorGrpcService;
        }

        public IEndpointRouteBuilder RegisterRoute(IEndpointRouteBuilder endpoints)
        {
            var authorGroup = endpoints.MapGroup("/api/authors").AddEndpointFilter<ApiExceptionFilter>();
            
            authorGroup.MapPost("/", CreateAuthor)

                .WithName("CreateAuthors")
                .WithDisplayName("Author Writes Endpoints")
                .WithTags("Authors")
                .Produces<CreateAuthorResponse>(201)
                .Produces<ValidationErrorResponse>(StatusCodes.Status422UnprocessableEntity)
                .Produces<ErrorResponse>(400)
                .Produces<ErrorResponse>(500);


            authorGroup.MapPut("/{id}", UpdateAuthor)
                 .AddEndpointFilter<GuidValidationFilter>()
                 .WithName("UpdateAuthor")
                .WithDisplayName("Author Writes Endpoints")
                .WithTags("Authors")
                .Produces(204)
                .Produces<ValidationErrorResponse>(StatusCodes.Status422UnprocessableEntity)
                .Produces<ErrorResponse>(400)
                .Produces<ErrorResponse>(500);
            
            authorGroup.MapDelete("/{id}", DeleteAuthor)
                .AddEndpointFilter<GuidValidationFilter>()
                .WithName("DeleteAuthor")
                .WithDisplayName("Author Writes Endpoints")
                .WithTags("Authors")
                .Produces(204)
                .Produces<ErrorResponse>(500);

            return authorGroup;
        }

        private async Task<IResult> CreateAuthor(CreateAuthorRequest author)
        {
            var addedAuthor = await _authorGrpcService.CreateAuthorAsync(author);
            return TypedResults.Ok(addedAuthor);
        }

        private async Task<IResult> UpdateAuthor(UpdateAuthorRequest author, string id)
        {
            await _authorGrpcService.UpdateAuthorAsync(author,id);
            return TypedResults.NoContent();
        }

        private async Task<IResult> DeleteAuthor(string id)
        {
            var request = new DeleteAuthorRequest (Guid.Parse(id));
            await _authorGrpcService.DeleteAuthorAsync(request);
            return TypedResults.NoContent();
        }
    }
}
