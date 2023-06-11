using MinimalApiCleanArchitecture.Domain.Model;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetAllBlogs
{
    public record GetAllBlogsResponse
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public DateTime CreatedDate { get; init; }
        public Author? Owner { get; init; }
    }
}
