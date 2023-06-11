using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;
using MinimalApiCleanArchitecture.Domain.Model;

namespace MinimalApiCleanArchitecture.Persistence.Repositories.Authors;

public class AuthorWriteRepository: GenericWriteRepository<Author>, IAuthorWriteRepository
{
    public AuthorWriteRepository(ThoughtfulDbContext dbContext) : base(dbContext)
    {
    }
}