namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;

public class CreateAuthorRequest
{
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? Bio { get; private set; }
    public DateTime DateOfBirth { get; private set; }

    public CreateAuthorRequest(string? firstName, string? lastName, string? bio, DateTime dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        Bio = bio;
        DateOfBirth = dateOfBirth;
    }
}