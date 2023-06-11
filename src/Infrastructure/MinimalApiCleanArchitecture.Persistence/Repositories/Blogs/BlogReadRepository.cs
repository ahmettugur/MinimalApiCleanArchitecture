using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Blogs;
using MinimalApiCleanArchitecture.Domain.Model;

namespace MinimalApiCleanArchitecture.Persistence.Repositories.Blogs;

public class BlogReadRepository: GenericReadRepository<Blog>,IBlogReadRepository
{
    public BlogReadRepository(ThoughtfulDbContext dbContext) : base(dbContext)
    {
    }
}