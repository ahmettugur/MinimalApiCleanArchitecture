using MinimalApiCleanArchitecture.Domain.Model;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.CreateBlog
{
    public record CreateBlogResponse
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public DateTime CreatedDate { get; init; }
        public Author? Owner { get; init; }
    }
}
