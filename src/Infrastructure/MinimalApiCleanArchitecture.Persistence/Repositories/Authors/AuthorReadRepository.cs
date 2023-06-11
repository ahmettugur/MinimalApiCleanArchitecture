using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;
using MinimalApiCleanArchitecture.Domain.Model;

namespace MinimalApiCleanArchitecture.Persistence.Repositories.Authors;

public class AuthorReadRepository: GenericReadRepository<Author>,IAuthorReadRepository
{
    public AuthorReadRepository(ThoughtfulDbContext dbContext) : base(dbContext)
    {
    }
}