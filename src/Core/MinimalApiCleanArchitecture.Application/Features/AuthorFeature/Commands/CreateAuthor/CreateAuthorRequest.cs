namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;


public record CreateAuthorRequest(string FirstName, string LastName, string Bio, DateTime DateOfBirth);