using MinimalApiCleanArchitecture.Domain.Entities;

namespace MinimalApiCleanArchitecture.Application.Interfaces.Repositories;

public interface IGenericWriteRepository<T> where T : BaseEntity
{
    Task<T> AddAsync(T entity);
    T Update(T entity);
    bool Remove(T entity);
    Task<int> SaveChangesAsync();
}