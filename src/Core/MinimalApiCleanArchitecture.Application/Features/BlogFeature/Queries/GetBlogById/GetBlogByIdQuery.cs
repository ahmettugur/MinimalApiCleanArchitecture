using MediatR;
using MinimalApiCleanArchitecture.Application.Common.Results;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetBlogById;

public class GetBlogByIdQuery : IRequest<IDataResult<GetBlogByIdResponse>>
{
    public Guid BlogId { get; private set; }

    public GetBlogByIdQuery(Guid blogId)
    {
        BlogId = blogId;
    }
}