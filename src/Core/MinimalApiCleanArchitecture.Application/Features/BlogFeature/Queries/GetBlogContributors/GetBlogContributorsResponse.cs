namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetBlogContributors
{
    public record GetBlogContributorsResponse
    {
        public Guid ContributorId { get; set; }
        public string? Name { get; set; }
        public string? Bio { get; set; }
    }
}
