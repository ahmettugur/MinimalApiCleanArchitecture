using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Blogs;
using MinimalApiCleanArchitecture.Persistence.Repositories.Authors;
using MinimalApiCleanArchitecture.Persistence.Repositories.Blogs;

namespace MinimalApiCleanArchitecture.Persistence;

public static class PersistenceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddTransient<IAuthorReadRepository, AuthorReadRepository>();
        services.AddTransient<IAuthorWriteRepository, AuthorWriteRepository>();
        services.AddTransient<IBlogReadRepository, BlogReadRepository>();
        services.AddTransient<IBlogWriteRepository, BlogWriteRepository>();
        
        services.AddDbContext<MinimalApiCleanArchitectureDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("Default")));

        return services;
    }

}