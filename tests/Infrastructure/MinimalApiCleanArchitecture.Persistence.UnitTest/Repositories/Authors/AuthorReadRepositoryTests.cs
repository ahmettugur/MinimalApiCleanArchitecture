using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;
using MinimalApiCleanArchitecture.Domain.Model;
using MinimalApiCleanArchitecture.Persistence.Repositories.Authors;
using Moq;

namespace MinimalApiCleanArchitecture.Persistence.UnitTest.Repositories.Authors;

public class AuthorReadRepositoryTests
{
    private readonly Mock<DbSet<Author>> _authorMock;
    private readonly Mock<MinimalApiCleanArchitectureDbContext> _contextMock;
    private readonly List<Author> _authors;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfigurationRoot _configuration;

    public AuthorReadRepositoryTests()
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
        
        _authors = new List<Author>
        {
            new()
            {
                Id  = Guid.NewGuid() ,FirstName = "Jon",LastName = "Doe",Bio = "Developer",DateOfBirth = new DateTime(1990,9,1)
            }
        };

        _authorMock = DbSetMock.CreateDbSetMock(_authors.AsQueryable());
    }
    
    
    [Fact]
    public void TestAddPersistenceServices_AddPersistenceServicesForAuthorShould_GetServices()
    {
        _serviceProvider.GetService<IAuthorReadRepository>().Should().NotBeNull();
        _serviceProvider.GetService<IAuthorWriteRepository>().Should().NotBeNull();
    }

    [Fact]
    public async Task TestAuthorReadRepositoryGetAllAuthors_AuthorReadRepositoryGetAllAuthorsParametersShouldReturn_GetAllAuthors()
    {
        _contextMock.Setup(x => x.Set<Author>()).Returns(_authorMock.Object);
        var repository = new AuthorReadRepository(_contextMock.Object);
        var result = await repository.GetAll();
        
        Assert.Equal(_authors, result!);
        _contextMock.Object.Authors.Should().NotBeNull();
    }

    [Fact]
    public async Task TestAuthorReadRepositoryGetAuthors_AuthorReadRepositoryGetAuthorsOrderByWithParametersShouldReturn_GetAllAuthors()
    {
        _contextMock.Setup(x => x.Set<Author>()).Returns(_authorMock.Object);
        var repository = new AuthorReadRepository(_contextMock.Object);
        var result = await repository.Get(false, x => x!.Id == _authors[0].Id,x => x.OrderBy(_=> _.DateOfBirth));

        Assert.Equal(_authors, result!);
    }

    [Fact]
    public async Task TestAuthorReadRepositoryGetAuthors_AuthorReadRepositoryGetAuthorsWithParametersShouldReturn_GetAllAuthors()
    {
        _contextMock.Setup(x => x.Set<Author>()).Returns(_authorMock.Object);
        var repository = new AuthorReadRepository(_contextMock.Object);
        var result = await repository.Get(false,x => x!.Id == _authors[0].Id);

        Assert.Equal(_authors, result!);
    }

    [Fact]
    public async Task TestAuthorReadRepositoryGetAuthorById_AuthorReadRepositoryGetAuthorByIdShouldReturn_GetAuthor()
    {
        var authorId = _authors[0].Id;

        var author = _authorMock.Object.FirstOrDefaultAsync(b => b.Id == authorId);
        _authorMock.Setup(m => m.FindAsync(authorId)).Returns(() => new ValueTask<Author?>(author));
    
        _contextMock.Setup(x => x.Set<Author>()).Returns(_authorMock.Object);
        var repository = new AuthorReadRepository(_contextMock.Object);
        var result = await repository.GetByIdAsync(authorId);
        result.Should().NotBeNull();
        result?.Id.Should().Be(authorId);
    }

    [Fact]
    public async Task TestAuthorReadRepositoryGetSingleAuthor_AuthorReadRepositoryGetSingleAuthorShouldReturn_GetAuthor()
    {
        var authorId = _authors[0].Id;

        var author = _authorMock.Object.FirstOrDefaultAsync(b => b.Id == authorId);
        _authorMock.Setup(m => m.FindAsync(authorId)).Returns(() => new ValueTask<Author?>(author));

        _contextMock.Setup(x => x.Set<Author>()).Returns(_authorMock.Object);
        var repository = new AuthorReadRepository(_contextMock.Object);
        var result = await repository.GetSingleAsync(_=> _.Id == authorId);
        result.Should().NotBeNull();
        result?.Id.Should().Be(authorId);
    }

}