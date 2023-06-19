using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.Domain.Model;
using MinimalApiCleanArchitecture.Persistence.Repositories.Authors;
using Moq;

namespace MinimalApiCleanArchitecture.Persistence.UnitTest.Repositories.Authors;

public class AuthorReadRepositoryTests
{
    private readonly Mock<MinimalApiCleanArchitectureDbContext> _contextMock;
    private readonly List<Author> _authors;


    public AuthorReadRepositoryTests()
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
        
        _authors = new List<Author>
        {
            new()
            {
                Id  = Guid.NewGuid() ,FirstName = "Jon",LastName = "Doe",Bio = "Developer",DateOfBirth = new DateTime(1990,9,1)
            }
        };
    }

    [Fact]
    public async Task TestGetAllAuthors_GetAllAuthorsParametersShouldReturn_GetAllAuthors()
    {
        var mockSettings = DbSetMock.CreateDbSetMock(_authors.AsQueryable());

        _contextMock.Setup(x => x.Set<Author>()).Returns(mockSettings.Object);
        var repository = new AuthorReadRepository(_contextMock.Object);
        var result = await repository.GetAll();
        
        Assert.Equal(_authors, result!);
    }

    [Fact]
    public async Task TestGetAuthors_GetAuthorsOrderByDateOfBirthWithParametersShouldReturn_GetAllAuthors()
    {
        var mockSettings = DbSetMock.CreateDbSetMock(_authors.AsQueryable());

        _contextMock.Setup(x => x.Set<Author>()).Returns(mockSettings.Object);
        var repository = new AuthorReadRepository(_contextMock.Object);
        var result = await repository.Get(false, x => x!.Id == _authors[0].Id,x => x.OrderBy(_=> _.DateOfBirth));

        Assert.Equal(_authors, result!);
    }

    [Fact]
    public async Task TestGetuthors_GetAuthorsWithParametersShouldReturn_GetAllAuthors()
    {

        var mockSettings = DbSetMock.CreateDbSetMock(_authors.AsQueryable());

        _contextMock.Setup(x => x.Set<Author>()).Returns(mockSettings.Object);
        var repository = new AuthorReadRepository(_contextMock.Object);
        var result = await repository.Get(false,x => x!.Id == _authors[0].Id);

        Assert.Equal(_authors, result!);
    }

    [Fact]
    public async Task TestGetAuthorById_GetAuthorByIdShouldReturn_GetAuthor()
    {
        var authorId = _authors[0].Id;

        var mockSettings = DbSetMock.CreateDbSetMock(_authors.AsQueryable());
        var author = mockSettings.Object.FirstOrDefaultAsync(b => b.Id == authorId);
        mockSettings.Setup(m => m.FindAsync(authorId)).Returns(() => new ValueTask<Author?>(author));
    
        _contextMock.Setup(x => x.Set<Author>()).Returns(mockSettings.Object);
        var repository = new AuthorReadRepository(_contextMock.Object);
        var result = await repository.GetByIdAsync(authorId);
        result.Should().NotBeNull();
        result?.Id.Should().Be(authorId);
    }

    [Fact]
    public async Task TestGetSingleAuthor_GetSingleAuthorShouldReturn_GetAuthor()
    {
        var authorId = _authors[0].Id;

        var mockSettings = DbSetMock.CreateDbSetMock(_authors.AsQueryable());
        var author = mockSettings.Object.FirstOrDefaultAsync(b => b.Id == authorId);
        mockSettings.Setup(m => m.FindAsync(authorId)).Returns(() => new ValueTask<Author?>(author));

        _contextMock.Setup(x => x.Set<Author>()).Returns(mockSettings.Object);
        var repository = new AuthorReadRepository(_contextMock.Object);
        var result = await repository.GetSingleAsync(_=> _.Id == authorId);
        result.Should().NotBeNull();
        result?.Id.Should().Be(authorId);
    }

}