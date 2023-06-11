namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.UpdateAuthor;

public class UpdateAuthorRequest
{
    public Guid AuthorId { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? Bio { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    
    public UpdateAuthorRequest(Guid authorId, string? firstName, string? lastName, string? bio, DateTime dateOfBirth)
    {
        AuthorId = authorId;
        FirstName = firstName;
        LastName = lastName;
        Bio = bio;
        DateOfBirth = dateOfBirth;
    }
}