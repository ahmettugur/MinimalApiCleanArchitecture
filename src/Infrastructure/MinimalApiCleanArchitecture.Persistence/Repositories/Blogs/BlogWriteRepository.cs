using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Blogs;
using MinimalApiCleanArchitecture.Domain.Model;

namespace MinimalApiCleanArchitecture.Persistence.Repositories.Blogs;

public class BlogWriteRepository: GenericWriteRepository<Blog>,IBlogWriteRepository
{
    public BlogWriteRepository(MinimalApiCleanArchitectureDbContext dbContext) : base(dbContext)
    {
    }
    
}