using MediatR;

namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.DeleteAuthor;

public class DeleteAuthorCommand : IRequest<DeleteAuthorResponse>
{
    public Guid AuthorId { get; private set; }
    
    public DeleteAuthorCommand(Guid authorId)
    {
        AuthorId = authorId;
    }
}
