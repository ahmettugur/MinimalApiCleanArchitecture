using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.DeleteAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.UpdateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAllAuthors;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAuthorById;

namespace MinimalApiCleanArchitecture.Application.Interfaces.GrpcServices.AuthorGrpc
{
    public interface IAuthorGrpcService
    {
        Task<List<GetAllAuthorsResponse>> GetAuthorsAsync();
        Task<GetAuthorByIdResponse> GetAuthorByIdAsync(string authorID);
        Task<CreateAuthorResponse> CreateAuthorAsync(CreateAuthorRequest author);
        Task<UpdateAuthorResponse> UpdateAuthorAsync(UpdateAuthorRequest author, string authorID);
        Task<DeleteAuthorResponse> DeleteAuthorAsync(DeleteAuthorRequest author);
    }
}
