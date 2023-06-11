using MediatR;
using MinimalApiCleanArchitecture.Application.Common.Results;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.AddContributor;

public class AddContributorCommand : IRequest<IResult>
{
    public Guid BlogId { get; private set; }
    public Guid ContributorId { get; private set; }

    public AddContributorCommand(Guid blogId, Guid contributorId)
    {
        BlogId = blogId;
        ContributorId = contributorId;
    }
}
