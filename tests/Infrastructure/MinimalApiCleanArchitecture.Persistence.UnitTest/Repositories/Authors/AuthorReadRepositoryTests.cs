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
    public async Task TestGetAllAuthors_GetAllAuthorsShouldReturn_GetAllAuthors()
    {
        
        var mockSettings = CreateDbSetMock(_authors.AsQueryable());

        _contextMock.Setup(x => x.Set<Author>()).Returns(mockSettings.Object);
        var repository = new AuthorReadRepository(_contextMock.Object);
        var result = await repository.GetAll();
        
        Assert.Equal(_authors, result!);
    }
    
    [Fact]
    public async Task TestGetAuthorById_GetAuthorByIdShouldReturn_GetAuthor()
    {
        var authorId = _authors[0].Id;

        var mockSettings = CreateDbSetMock(_authors.AsQueryable());
        mockSettings.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns((object[] _) =>
        {
            return new ValueTask<Author?>(mockSettings.Object.FirstOrDefaultAsync(b => b.Id == authorId));
        });
    
        _contextMock.Setup(x => x.Set<Author>()).Returns(mockSettings.Object);
        var repository = new AuthorReadRepository(_contextMock.Object);
        var result = await repository.GetByIdAsync(authorId);
        result.Should().NotBeNull();
        result?.Id.Should().Be(authorId);
    }
    
    
    private static Mock<DbSet<T>> CreateDbSetMock<T>(IQueryable<T> items) where T : class
    {
        var dbSetMock = new Mock<DbSet<T>>();

        dbSetMock.As<IAsyncEnumerable<T>>()
            .Setup(x => x.GetAsyncEnumerator(default))
            .Returns(new TestAsyncEnumerator<T>(items.GetEnumerator()));
        dbSetMock.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<T>(items.Provider));
        dbSetMock.As<IQueryable<T>>()
            .Setup(m => m.Expression).Returns(items.Expression);
        dbSetMock.As<IQueryable<T>>()
            .Setup(m => m.ElementType).Returns(items.ElementType);
        dbSetMock.As<IQueryable<T>>()
            .Setup(m => m.GetEnumerator()).Returns(items.GetEnumerator());

        return dbSetMock;
    }
}