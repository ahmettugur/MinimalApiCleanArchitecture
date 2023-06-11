namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.DeleteAuthor;


public class DeleteAuthorRequest
{
    public Guid AuthorId { get; private set; }
    
    public DeleteAuthorRequest(Guid authorId)
    {
        AuthorId = authorId;
    }
}