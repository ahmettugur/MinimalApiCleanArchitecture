using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.AddContributor;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Blogs;
using MinimalApiCleanArchitecture.Domain.Model;
using Moq;
using Xunit;

namespace MinimalApiCleanArchitecture.Application.UnitTests.Features.BlogFeature.Commands.AddContributor;

public class AddContributorCommandHandlerTests
{
    private readonly Mock<IBlogWriteRepository> _blogWriteRepository;
    private readonly Mock<IBlogReadRepository> _blogReadRepository;
    private readonly Mock<IAuthorReadRepository> _authorReadRepository;
    private AddContributorCommandHandler? _addContributorCommandHandler;
    
    private readonly List<Author> _authors;
    private readonly List<Blog> _blogs;

    public AddContributorCommandHandlerTests()
    {
        _blogWriteRepository = new Mock<IBlogWriteRepository>();
        _blogReadRepository = new Mock<IBlogReadRepository>();
        _authorReadRepository = new Mock<IAuthorReadRepository>();
        

        _authors = new List<Author>
        {
            new()
            {
                Id = Guid.NewGuid(), FirstName = "Jon", LastName = "Doe", Bio = "Developer",
                DateOfBirth = new DateTime(1990, 9, 1)
            }
        };
        _blogs = new List<Blog>
        {
            Blog.CreateBlog("C#", "C# Example", Guid.NewGuid())
        };
    }

    [Fact]
    public async Task TestAddContributor_AddContributorShouldReturn_SuccessDataResult()
    {
        var blog = _blogs[0];
        blog.Id = Guid.NewGuid();
        var author = _authors[0];
        _authorReadRepository.Setup(x => x.GetByIdAsync(author.Id, false)).ReturnsAsync(author);
        _blogReadRepository.Setup(x => x.GetByIdAsync(blog.Id, false)).ReturnsAsync(blog);

        _addContributorCommandHandler = new AddContributorCommandHandler(_blogReadRepository.Object,
            _blogWriteRepository.Object, _authorReadRepository.Object);

        var command = new AddContributorCommand(blog.Id, author.Id);
        var result = await _addContributorCommandHandler.Handle(command, CancellationToken.None);

        result.Success.Should().Be(true);
        blog.Contributors.Should().HaveCountGreaterThan(0);
        result.Message.Should()
            .Be(
                $"Contributor added successful with BlogId: {command.BlogId} and ContributorId: {command.ContributorId}");
        result = await _addContributorCommandHandler.Handle(command, CancellationToken.None);
        result.Success.Should().Be(true);


        blog.Contributors!.ToList()[0].FirstName.Should().Be("Jon");
        blog.Contributors!.ToList()[0].LastName.Should().Be("Doe");
        blog.Contributors!.ToList()[0].FullName.Should().Be("Jon Doe");
        blog.Contributors!.ToList()[0].Bio.Should().Be("Developer");
        blog.Contributors!.ToList()[0].DateOfBirth.Should().Be(new DateTime(1990, 9, 1));
    }

    [Fact]
    public void TestAddContributor_AddContributorNoneExistentAuthorShouldReturn_ThrowNotfoundException()
    {
        var blog = _blogs[0];
        blog.Id = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        _blogReadRepository.Setup(x => x.GetByIdAsync(blog.Id, false)).ReturnsAsync(blog);

        _addContributorCommandHandler = new AddContributorCommandHandler(_blogReadRepository.Object,
            _blogWriteRepository.Object, _authorReadRepository.Object);

        var command = new AddContributorCommand(blog.Id, authorId);

        Assert.ThrowsAnyAsync<NotFoundException>(() => _addContributorCommandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public void TestAddContributor_AddContributorNoneExistentBlogShouldReturn_ThrowNotfoundException()
    {
        var author = _authors[0];
        var blogId = Guid.NewGuid();
        _authorReadRepository.Setup(x => x.GetByIdAsync(author.Id, false)).ReturnsAsync(author);

        _addContributorCommandHandler = new AddContributorCommandHandler(_blogReadRepository.Object,
            _blogWriteRepository.Object, _authorReadRepository.Object);

        var command = new AddContributorCommand(blogId, author.Id);
        Assert.ThrowsAnyAsync<NotFoundException>(() => _addContributorCommandHandler.Handle(command, CancellationToken.None));
    }
}