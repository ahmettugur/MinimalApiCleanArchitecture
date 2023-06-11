namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;

public record CreateAuthorResponse
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Bio { get; init; }
    public DateTime DateOfBirth { get; init; }
}