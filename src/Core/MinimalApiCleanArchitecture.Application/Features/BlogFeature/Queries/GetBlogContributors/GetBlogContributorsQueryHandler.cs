using AutoMapper;
using MediatR;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Blogs;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetBlogContributors;

public class GetBlogContributorsQueryHandler : IRequestHandler<GetBlogContributorsQuery, IDataResult<List<GetBlogContributorsResponse>>>
{
    private readonly IBlogReadRepository _blogReadRepository;
    private readonly IMapper _mapper;

    public GetBlogContributorsQueryHandler(IBlogReadRepository blogReadRepository, IMapper mapper)
    {
        _blogReadRepository = blogReadRepository;
        _mapper = mapper;
    }

    public async Task<IDataResult<List<GetBlogContributorsResponse>>> Handle(GetBlogContributorsQuery request, CancellationToken cancellationToken)
    {
        var blog = await _blogReadRepository.GetByIdAsync(request.BlogId,true,_ => _!.Contributors);
        if (blog is null)
        {
            return new ErrorDataResult<List<GetBlogContributorsResponse>>(
                $"Contributor cannot found with blog id: {request.BlogId}");
        }
        var mappedBlog = _mapper.Map<List<GetBlogContributorsResponse>>(blog.Contributors);
        var result = new SuccessDataResult<List<GetBlogContributorsResponse>>(mappedBlog, "Success");
        return result;
    }
}