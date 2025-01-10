using AutoMapper;
using Microsoft.Extensions.Logging;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.DeleteAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.UpdateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAllAuthors;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAuthorById;
using MinimalApiCleanArchitecture.Application.Interfaces.GrpcServices.AuthorGrpc;
using MinimalApiCleanArchitecture.Infrastructure.Protos;
using Newtonsoft.Json;

namespace MinimalApiCleanArchitecture.Infrastructure.Services.GrpcServices.AuthorGrpc
{
    public class AuthorGrpcService : IAuthorGrpcService
    {
        private readonly AuthorProtoService.AuthorProtoServiceClient _authorProtoService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorGrpcService> _logger;
        public AuthorGrpcService(AuthorProtoService.AuthorProtoServiceClient authorProtoService, IMapper mapper,ILogger<AuthorGrpcService> logger)
        {
            _authorProtoService = authorProtoService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<GetAllAuthorsResponse>> GetAuthorsAsync()
        {
            var result = await _authorProtoService.GetAuthorsAsync(new Google.Protobuf.WellKnownTypes.Empty());
            var authors = _mapper.Map<List<GetAllAuthorsResponse>>(result.Authors);
            return authors;
        }
        public async Task<GetAuthorByIdResponse> GetAuthorByIdAsync(string authorId)
        {
            var result = await _authorProtoService.GetAuthorByIdAsync(new GetAuthorByIdProtoRequest { AuthorId = authorId });
            var author = _mapper.Map<GetAuthorByIdResponse>(result.Author);
            return author;
        }
        public async Task<CreateAuthorResponse> CreateAuthorAsync(CreateAuthorRequest author)
        {
            var request = _mapper.Map<CreateAuthorProtoRequest>(author);
            var result = await _authorProtoService.CreateAuthorAsync(request);
            var addedAuthor = _mapper.Map<CreateAuthorResponse>(result.Author);
            
            _logger.LogInformation("Author has been created. {Response}",JsonConvert.SerializeObject(addedAuthor));
            
            return addedAuthor;
        }
        public async Task<UpdateAuthorResponse> UpdateAuthorAsync(UpdateAuthorRequest author, string authorId)
        {
            var request = _mapper.Map<UpdateAuthorProtoRequest>(author);
            request.Id = authorId;
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
