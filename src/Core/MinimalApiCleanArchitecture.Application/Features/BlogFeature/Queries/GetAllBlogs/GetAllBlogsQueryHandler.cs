using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Blogs;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetAllBlogs;

public class GetAllBlogsQueryHandler : IRequestHandler<GetAllBlogsQuery, IDataResult<List<GetAllBlogsResponse>>>
{
    private readonly ILogger<GetAllBlogsQueryHandler> _logger;
    private readonly IBlogReadRepository _blogReadRepository;
    private readonly IMapper _mapper;

    public GetAllBlogsQueryHandler(IBlogReadRepository blogReadRepository, IMapper mapper, ILogger<GetAllBlogsQueryHandler> logger)
    {
        _blogReadRepository = blogReadRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IDataResult<List<GetAllBlogsResponse>>> Handle(GetAllBlogsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetAll Query Handle");
        var blogs = await _blogReadRepository.Get(true,null, null, _ => _!.Owner);
        var mappedBlogs = _mapper.Map<List<GetAllBlogsResponse>>(blogs);
        var result = new SuccessDataResult<List<GetAllBlogsResponse>>(mappedBlogs, "Success");
        return result;
    }
}