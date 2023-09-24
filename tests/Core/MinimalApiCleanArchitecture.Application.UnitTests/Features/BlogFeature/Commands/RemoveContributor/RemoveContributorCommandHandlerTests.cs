using FluentAssertions;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.AddContributor;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.RemoveContributor;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Blogs;
using MinimalApiCleanArchitecture.Domain.Model;
using Moq;
using Xunit;

namespace MinimalApiCleanArchitecture.Application.UnitTests.Features.BlogFeature.Commands.RemoveContributor
{
    public class RemoveContributorCommandHandlerTests
    {
        private readonly Mock<IBlogWriteRepository> _blogWriteRepository;
        private readonly Mock<IBlogReadRepository> _blogReadRepository;
        private readonly Mock<IAuthorReadRepository> _authorReadRepository;
        private readonly List<Author> _authors;
        private readonly List<Blog> _blogs;
        private RemoveContributorCommandHandler? _removeContributorCommandHandler;
        private AddContributorCommandHandler? _addContributorCommandHandler;
        public RemoveContributorCommandHandlerTests()
        {
            _blogWriteRepository = new Mock<IBlogWriteRepository>();
            _blogReadRepository = new Mock<IBlogReadRepository>();
            _authorReadRepository = new Mock<IAuthorReadRepository>();

            _authors = new List<Author>
            {
                new()
                {
                    Id  = Guid.NewGuid() ,FirstName = "Jon",LastName = "Doe",Bio = "Developer",DateOfBirth = new DateTime(1990,9,1)
                }
            };
            _blogs = new List<Blog>
            {
               Blog.CreateBlog("C#","C# Example",Guid.NewGuid())
            };
        }

        [Fact]
        public async Task TestAddAndRemoveContributor_RemoveContributorShouldReturn_NoException()
        {
            var blog = _blogs[0];
            blog.Id = Guid.NewGuid();
            var author = _authors[0];

            _authorReadRepository.Setup(x => x.GetByIdAsync(author.Id, false)).ReturnsAsync(author);
            _blogReadRepository.Setup(x => x.GetByIdAsync(blog.Id, false)).ReturnsAsync(blog);


            _addContributorCommandHandler = new AddContributorCommandHandler(_blogReadRepository.Object, _blogWriteRepository.Object, _authorReadRepository.Object);

            var addContributorCommand = new AddContributorCommand(blog.Id, author.Id);
            var addContributorResult = await _addContributorCommandHandler.Handle(addContributorCommand, CancellationToken.None);

            addContributorResult.Success.Should().Be(true);
            blog.Contributors.Should().HaveCountGreaterThan(0);


            _blogReadRepository.Setup(x => x.GetByIdAsync(blog.Id, false, b => b!.Contributors)).ReturnsAsync(blog);

            var removeContributorCommand = new RemoveContributorCommand(blog.Id, author.Id);
            _removeContributorCommandHandler = new RemoveContributorCommandHandler(_blogReadRepository.Object, _blogWriteRepository.Object, _authorReadRepository.Object);

            var removeContributorResult = await _removeContributorCommandHandler.Handle(removeContributorCommand, CancellationToken.None);
            removeContributorResult.Success.Should().Be(true);
            blog.Contributors.Should().HaveCount(0);

        }


        [Fact]
        public async Task TestAddAndRemoveContributor_RemoveContributorShouldReturn_AuthorNotFoundException()
        {
            var blog = _blogs[0];
            blog.Id = Guid.NewGuid();
            var author = _authors[0];

            _authorReadRepository.Setup(x => x.GetByIdAsync(author.Id, false)).ReturnsAsync(author);
            _blogReadRepository.Setup(x => x.GetByIdAsync(blog.Id, false)).ReturnsAsync(blog);


            _addContributorCommandHandler = new AddContributorCommandHandler(_blogReadRepository.Object, _blogWriteRepository.Object, _authorReadRepository.Object);

            var addContributorCommand = new AddContributorCommand(blog.Id, author.Id);
            var addContributorResult = await _addContributorCommandHandler.Handle(addContributorCommand, CancellationToken.None);

            addContributorResult.Success.Should().Be(true);
            blog.Contributors.Should().HaveCountGreaterThan(0);

            _blogReadRepository.Setup(x => x.GetByIdAsync(blog.Id, false, b => b!.Contributors)).ReturnsAsync(blog);

            author.Id = Guid.NewGuid();
            var removeContributorCommand = new RemoveContributorCommand(blog.Id, author.Id);
            _removeContributorCommandHandler = new RemoveContributorCommandHandler(_blogReadRepository.Object, _blogWriteRepository.Object, _authorReadRepository.Object);
            
            await Assert.ThrowsAnyAsync<NotFoundException>( () => _removeContributorCommandHandler.Handle(removeContributorCommand, CancellationToken.None));
            
        }

        [Fact]
        public async Task TestAddAndRemoveContributor_RemoveContributorShouldReturn_BlogNotFoundException()
        {
            var blog = _blogs[0];
            blog.Id = Guid.NewGuid();
            var author = _authors[0];

            _authorReadRepository.Setup(x => x.GetByIdAsync(author.Id, false)).ReturnsAsync(author);
            _blogReadRepository.Setup(x => x.GetByIdAsync(blog.Id, false)).ReturnsAsync(blog);


            _addContributorCommandHandler = new AddContributorCommandHandler(_blogReadRepository.Object, _blogWriteRepository.Object, _authorReadRepository.Object);

            var addContributorCommand = new AddContributorCommand(blog.Id, author.Id);
            var addContributorResult = await _addContributorCommandHandler.Handle(addContributorCommand, CancellationToken.None);

            addContributorResult.Success.Should().Be(true);
            blog.Contributors.Should().HaveCountGreaterThan(0);

            _blogReadRepository.Setup(x => x.GetByIdAsync(blog.Id, false, b => b!.Contributors)).ReturnsAsync(blog);

            blog.Id = Guid.NewGuid();
            var removeContributorCommand = new RemoveContributorCommand(blog.Id, author.Id);
            _removeContributorCommandHandler = new RemoveContributorCommandHandler(_blogReadRepository.Object, _blogWriteRepository.Object, _authorReadRepository.Object);
            await Assert.ThrowsAnyAsync<NotFoundException>( () => _removeContributorCommandHandler.Handle(removeContributorCommand, CancellationToken.None));
     
        }
    }
}
