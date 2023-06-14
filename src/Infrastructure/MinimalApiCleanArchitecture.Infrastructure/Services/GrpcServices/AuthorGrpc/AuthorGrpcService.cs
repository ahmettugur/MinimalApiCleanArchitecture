using AutoMapper;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.DeleteAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.UpdateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAllAuthors;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAuthorById;
using MinimalApiCleanArchitecture.Application.Interfaces.GrpcServices.AuthorGrpc;
using MinimalApiCleanArchitecture.Infrastructure.Protos;

namespace MinimalApiCleanArchitecture.Infrastructure.Services.GrpcServices.AuthorGrpc
{
    public class AuthorGrpcService : IAuthorGrpcService
    {
        private readonly AuthorProtoService.AuthorProtoServiceClient _authorProtoService;
        private readonly IMapper _mapper;
        public AuthorGrpcService(AuthorProtoService.AuthorProtoServiceClient authorProtoService, IMapper mapper)
        {
            _authorProtoService = authorProtoService;
            _mapper = mapper;
        }

        public async Task<List<GetAllAuthorsResponse>> GetAuthorsAsync()
        {
            var result = await _authorProtoService.GetAuthorsAsync(new Google.Protobuf.WellKnownTypes.Empty());
            var authors = _mapper.Map<List<GetAllAuthorsResponse>>(result.Authors);
            return authors;
        }
        public async Task<GetAuthorByIdResponse> GetAuthorByIdAsync(string authorID)
        {
            var result = await _authorProtoService.GetAuthorByIdAsync(new GetAuthorByIdProtoRequest { AuthorId = authorID });
            var author = _mapper.Map<GetAuthorByIdResponse>(result.Author);
            return author;
        }
        public async Task<CreateAuthorResponse> CreateAuthorAsync(CreateAuthorRequest author)
        {
            var request = _mapper.Map<CreateAuthorProtoRequest>(author);
            var result = await _authorProtoService.CreateAuthorAsync(request);
            var addedAuthor = _mapper.Map<CreateAuthorResponse>(result.Author);
            return addedAuthor;
        }
        public async Task<UpdateAuthorResponse> UpdateAuthorAsync(UpdateAuthorRequest author, string authorID)
        {
            var request = _mapper.Map<UpdateAuthorProtoRequest>(author);
            request.Id = authorID;
            var response = await _authorProtoService.UpdateAuthorAsync(request);
            var updatedAuthor = _mapper.Map<UpdateAuthorResponse>(response);
            return updatedAuthor;
        }

        public async Task<DeleteAuthorResponse> DeleteAuthorAsync(DeleteAuthorRequest author)
        {
            var request = _mapper.Map<DeleteAuthorProtoRequest>(author);
            var response = await _authorProtoService.DeleteAuthorAsync(request);
            var deletedAuthor = _mapper.Map<DeleteAuthorResponse>(response);
            return deletedAuthor;
        }
    }
}
