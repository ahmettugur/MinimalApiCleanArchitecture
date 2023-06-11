using AutoMapper;
using MediatR;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;

namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAllAuthors;

public class GetAllAuthorsQueryHandler : IRequestHandler<GetAllAuthorsQuery, List<GetAllAuthorsResponse>>
{
    private readonly IAuthorReadRepository _authorReadRepository;
    private readonly IMapper _mapper;
    public GetAllAuthorsQueryHandler(IAuthorReadRepository authorReadRepository ,IMapper mapper)
    {

        _authorReadRepository = authorReadRepository;       
        _mapper = mapper;

    }

    public async Task<List<GetAllAuthorsResponse>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
    {
        var authors = await _authorReadRepository.GetAll();
        var mappedAuthors = _mapper.Map<List<GetAllAuthorsResponse>>(authors);
        return mappedAuthors;
    }
}