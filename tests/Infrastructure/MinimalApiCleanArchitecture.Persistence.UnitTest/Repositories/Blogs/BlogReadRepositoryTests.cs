using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Blogs;
using MinimalApiCleanArchitecture.Domain.Model;
using MinimalApiCleanArchitecture.Persistence.Repositories.Blogs;
using Moq;

namespace MinimalApiCleanArchitecture.Persistence.UnitTest.Repositories.Blogs;

public class BlogReadRepositoryTests
{
    private readonly Mock<DbSet<Blog>> _blogMock;
    private readonly Mock<MinimalApiCleanArchitectureDbContext> _contextMock;
    private readonly List<Blog> _blogs;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfigurationRoot _configuration;

    public BlogReadRepositoryTests()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        
        IServiceCollection services = new ServiceCollection();
        services.AddPersistenceServices(_configuration);
        services.AddLogging();

        _serviceProvider = services.BuildServiceProvider();

        _contextMock = new Mock<MinimalApiCleanArchitectureDbContext>();

        _blogs = new List<Blog>
        {
            Blog.CreateBlog("C#","C# Example",Guid.NewGuid())
        };

        _blogMock = DbSetMock.CreateDbSetMock(_blogs.AsQueryable());
    }

    [Fact]
    public void TestAddPersistenceServices_AddPersistenceServicesForBlogShould_GetServices()
    {
        _serviceProvider.GetService<IBlogReadRepository>().Should().NotBeNull();
        _serviceProvider.GetService<IBlogWriteRepository>().Should().NotBeNull();

    }
    
    [Fact]
    public async Task TestBlogReadRepositoryGetAllBlogs_BlogReadRepositoryGetAllBlogsParametersShouldReturn_GetAllBlogs()
    {
        _contextMock.Setup(x => x.Set<Blog>()).Returns(_blogMock.Object);
        var repository = new BlogReadRepository(_contextMock.Object);
        var result = await repository.GetAll(true);
        
        Assert.Equal(_blogs, result!);
        
        _contextMock.Object.Blogs.Should().NotBeNull();
    }

    [Fact]
    public async Task TestBlogReadRepositoryGetBlogs_BlogReadRepositoryGetBlogsOrderByWithParametersShouldReturn_GetAllBlogs()
    {
        _contextMock.Setup(x => x.Set<Blog>()).Returns(_blogMock.Object);
        var repository = new BlogReadRepository(_contextMock.Object);
        var result = await repository.Get(true, x => x!.Id == _blogs[0].Id,x => x.OrderBy(_=> _.CreatedDate),_=> _.Contributors);

        Assert.Equal(_blogs, result!);
    }

    [Fact]
    public async Task TestBlogReadRepositoryGetBlogs_BlogReadRepositoryGetBlogsWithParametersShouldReturn_GetAllBlogs()
    {
        _contextMock.Setup(x => x.Set<Blog>()).Returns(_blogMock.Object);
        var repository = new BlogReadRepository(_contextMock.Object);
        var result = await repository.Get(false,x => x!.Id == _blogs[0].Id);

        Assert.Equal(_blogs, result!);
    }

    [Fact]
    public async Task TestBlogReadRepositoryGetBlogById_BlogReadRepositoryGetBlogByIdShouldReturn_GetBlog()
    {
        var blogId = _blogs[0].Id;

        var blog = _blogMock.Object.FirstOrDefaultAsync(b => b.Id == blogId);
        _blogMock.Setup(m => m.FindAsync(blogId)).Returns(() => new ValueTask<Blog?>(blog));
    
        _contextMock.Setup(x => x.Set<Blog>()).Returns(_blogMock.Object);
        var repository = new BlogReadRepository(_contextMock.Object);
        var result = await repository.GetByIdAsync(blogId,true,_=> _.Contributors);
        result.Should().NotBeNull();
        result?.Id.Should().Be(blogId);
    }

    [Fact]
    public async Task TestBlogReadRepositoryGetSingleBlog_BlogReadRepositoryGetSingleBlogShouldReturn_GetBlog()
    {
        var blogId = _blogs[0].Id;

        var blog = _blogMock.Object.FirstOrDefaultAsync(b => b.Id == blogId);
        _blogMock.Setup(m => m.FindAsync(blogId)).Returns(() => new ValueTask<Blog?>(blog));

        _contextMock.Setup(x => x.Set<Blog>()).Returns(_blogMock.Object);
        var repository = new BlogReadRepository(_contextMock.Object);
        var result = await repository.GetSingleAsync(_=> _.Id == blogId,true,_=> _.Contributors);
        result.Should().NotBeNull();
        result?.Id.Should().Be(blogId);
        
        
    }

}