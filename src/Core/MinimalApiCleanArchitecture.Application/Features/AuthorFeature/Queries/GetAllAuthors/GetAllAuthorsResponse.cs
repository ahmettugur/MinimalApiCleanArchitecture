namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAllAuthors;

public record GetAllAuthorsResponse
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Bio { get; init; }
    public DateTime DateOfBirth { get; init; }
}
