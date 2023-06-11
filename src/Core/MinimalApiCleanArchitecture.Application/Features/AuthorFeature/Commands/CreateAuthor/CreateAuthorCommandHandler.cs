using AutoMapper;
using MediatR;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;
using MinimalApiCleanArchitecture.Domain.Model;

namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;

public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, CreateAuthorResponse>
{
    private readonly IMapper _mapper;
    private readonly IAuthorWriteRepository _authorWriteRepository;

    public CreateAuthorCommandHandler(IAuthorWriteRepository authorWriteRepository, IMapper mapper)
    {
        _authorWriteRepository = authorWriteRepository;        
        _mapper = mapper;

    }
    public async Task<CreateAuthorResponse> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentException("Author cannot be null");
        }
        var author = _mapper.Map<Author>(request);

        await _authorWriteRepository.AddAsync(author);
        await _authorWriteRepository.SaveChangesAsync();

        var mappedAuthor = _mapper.Map<CreateAuthorResponse>(author);
        return mappedAuthor;
    }
}