using MediatR;
using MinimalApiCleanArchitecture.Application.Common.Results;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.RemoveContributor;

public class RemoveContributorCommand : IRequest<IResult>
{
    public Guid BlogId { get; private set; }
    public Guid ContributorId { get; private set; }

    public RemoveContributorCommand(Guid blogId, Guid contributorId)
    {
        BlogId = blogId;
        ContributorId = contributorId;
    }
}