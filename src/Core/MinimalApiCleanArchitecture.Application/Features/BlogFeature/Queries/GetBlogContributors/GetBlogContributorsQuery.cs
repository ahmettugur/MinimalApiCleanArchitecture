using MediatR;
using MinimalApiCleanArchitecture.Application.Common.Results;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetBlogContributors;

public class GetBlogContributorsQuery : IRequest<IDataResult<List<GetBlogContributorsResponse>>>
{
    public Guid BlogId { get; private set; }

    public GetBlogContributorsQuery(Guid blogId)
    {
        BlogId = blogId;
    }
}