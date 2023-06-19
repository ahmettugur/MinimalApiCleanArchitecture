using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.Domain.Model;
using MinimalApiCleanArchitecture.Persistence.Repositories.Authors;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalApiCleanArchitecture.Persistence.UnitTest.Repositories.Authors;

public class AuthorWriteRepositoryTests
{
    private readonly Mock<DbSet<Author>> _authorMock;
    private readonly Mock<MinimalApiCleanArchitectureDbContext> _contextMock;
    private readonly List<Author> _authors;


    public AuthorWriteRepositoryTests()
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

        _authorMock = DbSetMock.CreateDbSetMock(_authors.AsQueryable());
    }

    [Fact]
    public async Task TestAuthorWriteRepositoryCreateAuthor_AuthorWriteRepositoryCreateAuthorShouldReturn_InsertedAuthor()
    {
        var author = _authors[0];
        var authors = new List<Author>();
        _authorMock.Setup(m => m.AddAsync(It.IsAny<Author>(), default)).Callback<Author, CancellationToken>((_, _) =>
        {
            authors.Add(author);
        });

        _contextMock.Setup(x => x.Set<Author>()).Returns(_authorMock.Object);
        var repository = new AuthorWriteRepository(_contextMock.Object);
        var result = await repository.AddAsync(author);
        await repository.SaveChangesAsync();

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task TestAuthorWriteRepositoryUpdateAuthor_UpdateAuthorWriteRepositoryAuthorShouldReturn_UpdateStstusTrue()
    {
        var author = _authors[0];
        author.FirstName = "Jonn";

        _contextMock.Setup(x => x.Set<Author>()).Returns(_authorMock.Object);
        var repository = new AuthorWriteRepository(_contextMock.Object);

        var result = repository.Update(author);
        await repository.SaveChangesAsync();

        result.FirstName.Should().Be(author.FirstName);
    }

    [Fact]
    public async Task TestAuthorWriteRepositoryDeleteAuthor_AuthorWriteRepositoryDeleteAuthorShouldReturn_DeleteStatusTrue()
    {
        var author = _authors[0];

        _contextMock.Setup(x => x.Set<Author>()).Returns(_authorMock.Object);
        var repository = new AuthorWriteRepository(_contextMock.Object);

        var result = repository.Remove(author);
        await repository.SaveChangesAsync();

        result.Should().BeTrue();
    }

}
