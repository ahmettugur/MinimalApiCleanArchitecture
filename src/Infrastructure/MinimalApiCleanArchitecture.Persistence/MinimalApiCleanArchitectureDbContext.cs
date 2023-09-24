using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MinimalApiCleanArchitecture.Domain.Model;

namespace MinimalApiCleanArchitecture.Persistence;

public class MinimalApiCleanArchitectureDbContext : DbContext
{
    public MinimalApiCleanArchitectureDbContext()
    {

    }

    public MinimalApiCleanArchitectureDbContext(DbContextOptions options) : base(options)
    {

    }
    
    public DbSet<Blog> Blogs { get; set; } = null!;
    public DbSet<Author> Authors { get; set; } = null!;
}

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MinimalApiCleanArchitectureDbContext>
{
    public MinimalApiCleanArchitectureDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("Default");
        var builder = new DbContextOptionsBuilder<MinimalApiCleanArchitectureDbContext>();
        if (string.IsNullOrEmpty(connectionString))
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = configuration.GetConnectionString("Default");
        }
        builder.UseSqlServer(connectionString);
        return new MinimalApiCleanArchitectureDbContext(builder.Options);
    }
}