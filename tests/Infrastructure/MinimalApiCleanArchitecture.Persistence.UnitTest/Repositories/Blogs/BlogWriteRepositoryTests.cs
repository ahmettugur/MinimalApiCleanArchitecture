using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.Domain.Model;
using MinimalApiCleanArchitecture.Persistence.Repositories.Blogs;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalApiCleanArchitecture.Persistence.UnitTest.Repositories.Blogs;

public class BlogWriteRepositoryTests
{
    private readonly Mock<DbSet<Blog>> _blogMock;
    private readonly Mock<MinimalApiCleanArchitectureDbContext> _contextMock;
    private readonly List<Blog> _blogs;


    public BlogWriteRepositoryTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        IServiceCollection services = new ServiceCollection();
        services.AddPersistenceServices(configuration);
        services.AddLogging();

        services.BuildServiceProvider();

        _contextMock = new Mock<MinimalApiCleanArchitectureDbContext>();

        _blogs = new List<Blog>
        {
            Blog.CreateBlog("C#","C# Example",Guid.NewGuid())
        };

        _blogMock = DbSetMock.CreateDbSetMock(_blogs.AsQueryable());
    }

    [Fact]
    public async Task TestBlogWriteRepositoryCreateBlog_BlogWriteRepositoryCreateBlogShouldReturn_InsertedBlog()
    {
        var blog = _blogs[0];
        var blogs = new List<Blog>();
        _blogMock.Setup(m => m.AddAsync(It.IsAny<Blog>(), default)).Callback<Blog, CancellationToken>((_, _) =>
        {
            blogs.Add(blog);
        });

        _contextMock.Setup(x => x.Set<Blog>()).Returns(_blogMock.Object);
        var repository = new BlogWriteRepository(_contextMock.Object);
        var result = await repository.AddAsync(blog);
        await repository.SaveChangesAsync();

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task TestBlogWriteRepositoryUpdateBlog_UpdateBlogWriteRepositoryBlogShouldReturn_UpdateStstusTrue()
    {
        var blog = _blogs[0];

        blog.UpdateBlog("Test", " Evensourcing", blog.AuthorId);

        _contextMock.Setup(x => x.Set<Blog>()).Returns(_blogMock.Object);
        var repository = new BlogWriteRepository(_contextMock.Object);

        var result = repository.Update(blog);
        await repository.SaveChangesAsync();

        result.Name.Should().Be(blog.Name);
    }

    [Fact]
    public async Task TestBlogWriteRepositoryDeleteBlog_BlogWriteRepositoryDeleteBlogShouldReturn_DeleteStatusTrue()
    {
        var blog = _blogs[0];

        _contextMock.Setup(x => x.Set<Blog>()).Returns(_blogMock.Object);
        var repository = new BlogWriteRepository(_contextMock.Object);

        var result = repository.Remove(blog);
        await repository.SaveChangesAsync();

        result.Should().BeTrue();
    }

}
