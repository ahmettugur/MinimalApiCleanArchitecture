namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAuthorById;

public record GetAuthorByIdResponse
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Bio { get; init; }
    public DateTime DateOfBirth { get; init; }
}
