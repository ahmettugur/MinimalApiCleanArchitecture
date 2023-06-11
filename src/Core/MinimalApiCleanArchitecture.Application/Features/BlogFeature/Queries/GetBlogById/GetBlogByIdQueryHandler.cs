using AutoMapper;
using MediatR;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Blogs;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetBlogById;

public class GetBlogByIdQueryHandler : IRequestHandler<GetBlogByIdQuery, IDataResult<GetBlogByIdResponse>>
{
    private readonly IBlogReadRepository _blogReadRepository;
    private readonly IMapper _mapper;

    public GetBlogByIdQueryHandler(IBlogReadRepository blogReadRepository, IMapper mapper)
    {
        _blogReadRepository = blogReadRepository;
        _mapper = mapper;
    }

    public async Task<IDataResult<GetBlogByIdResponse>> Handle(GetBlogByIdQuery request, CancellationToken cancellationToken)
    {
        var blog = await _blogReadRepository.GetByIdAsync(request.BlogId,true,_ => _!.Owner);
        if (blog is null)
        {
            return new ErrorDataResult<GetBlogByIdResponse>($"Blog cannot found with id: {request.BlogId}");
        }
        var mappedBlog = _mapper.Map<GetBlogByIdResponse>(blog);
        var result = new SuccessDataResult<GetBlogByIdResponse>(mappedBlog, "Success");
        return result;
    }
}