using AutoMapper;
using MediatR;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;

namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAuthorById;

public class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, GetAuthorByIdResponse>
{
    private readonly IAuthorReadRepository _authorReadRepository;
    private readonly IMapper _mapper;

    public GetAuthorByIdQueryHandler(IAuthorReadRepository authorReadRepository,IMapper mapper)
    {
        _authorReadRepository = authorReadRepository;
        _mapper = mapper;
    }

    public async Task<GetAuthorByIdResponse> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        var author = await _authorReadRepository.GetByIdAsync(request.AuthorId);
        if (author is null)
        {
            throw new NotFoundException($"Author cannot found with id: {request.AuthorId}");
        }
        return _mapper.Map<GetAuthorByIdResponse>(author);
    }
}