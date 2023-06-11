using MinimalApiCleanArchitecture.Application.Interfaces.Repositories;
using MinimalApiCleanArchitecture.Domain.Entities;

namespace MinimalApiCleanArchitecture.Persistence.Repositories;

public class GenericWriteRepository<T> : IGenericWriteRepository<T> where T : BaseEntity
{
    private readonly MinimalApiCleanArchitectureDbContext dbContext;

    public GenericWriteRepository(MinimalApiCleanArchitectureDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await dbContext.Set<T>().AddAsync(entity);
        return entity;
    }

    public virtual T Update(T entity)
    {
        dbContext.Set<T>().Update(entity);
        return entity;
    }

    public virtual bool Remove(T entity)
    {
        dbContext.Set<T>().Remove(entity);
        return true;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync();
    }
}