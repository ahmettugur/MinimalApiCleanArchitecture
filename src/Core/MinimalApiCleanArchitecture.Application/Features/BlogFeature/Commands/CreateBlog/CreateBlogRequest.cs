namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.CreateBlog
{
    public record CreateBlogRequest(string Name, string Description, Guid AuthorId);
}
