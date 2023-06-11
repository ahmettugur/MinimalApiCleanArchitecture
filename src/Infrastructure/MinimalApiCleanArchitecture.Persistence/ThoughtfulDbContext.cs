using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MinimalApiCleanArchitecture.Domain.Model;

namespace MinimalApiCleanArchitecture.Persistence;

public class ThoughtfulDbContext : DbContext
{
    public ThoughtfulDbContext(DbContextOptions options) : base(options)
    {

    }
    
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Author> Authors { get; set; }
}

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ThoughtfulDbContext>
{
    public ThoughtfulDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("Default");
        var builder = new DbContextOptionsBuilder<ThoughtfulDbContext>();
        if (string.IsNullOrEmpty(connectionString))
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile(@Directory.GetCurrentDirectory() + "/../Thoughtful.Api/appsettings.json")
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = configuration.GetConnectionString("Default");
        }
        builder.UseSqlServer(connectionString);
        return new ThoughtfulDbContext(builder.Options);
    }
}