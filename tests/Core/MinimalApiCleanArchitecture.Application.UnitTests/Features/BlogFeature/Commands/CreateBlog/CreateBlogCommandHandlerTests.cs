using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.CreateBlog;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Blogs;
using MinimalApiCleanArchitecture.Domain.Model;
using Moq;
using Xunit;

namespace MinimalApiCleanArchitecture.Application.UnitTests.Features.BlogFeature.Commands.CreateBlog;

public class CreateBlogCommandHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IBlogWriteRepository> _blogBlogWriteRepository;
    private readonly Mock<IAuthorReadRepository> _authorReadRepository;
    private CreateBlogCommandHandler? _createBlogCommandHandler;
    private readonly IServiceCollection _services;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<Author> _authors;
    private readonly CreateBlogValidator _createBlogValidator;

    public CreateBlogCommandHandlerTests()
    {
        _blogBlogWriteRepository = new Mock<IBlogWriteRepository>();
        _authorReadRepository = new Mock<IAuthorReadRepository>();
        _services = new ServiceCollection();
        _services.AddApplicationServices();
        _services.AddLogging();

        _serviceProvider = _services.BuildServiceProvider();
        _mapper = _serviceProvider.GetService<IMapper>()!;
        _createBlogValidator = new CreateBlogValidator();
        
        _authors = new List<Author>
        {
            new()
            {
                Id  = Guid.NewGuid() ,FirstName = "Jon",LastName = "Doe",Bio = "Developer",DateOfBirth = new DateTime(1990,9,1)
            }
        };
    }

    [Fact]
    public async Task TestCreateBlog_CreateBlogWithValidCommandShouldReturn_SuccessDataResult()
    {
        var author = _authors[0];
        var authorID = author.Id;
        _authorReadRepository.Setup(x => x.GetByIdAsync(authorID,false)).ReturnsAsync(author);
        _createBlogCommandHandler = new CreateBlogCommandHandler(_blogBlogWriteRepository.Object, _authorReadRepository.Object, _mapper);

        var createBlogRequest = new CreateBlogRequest("C#", "AspNet", authorID);
        
        var command = _mapper.Map<CreateBlogCommand>(createBlogRequest);
        var validationResult = await _createBlogValidator.ValidateAsync(command);
        validationResult.Errors.Count.Should().Be(0);
        
        var result = await _createBlogCommandHandler.Handle(command, CancellationToken.None);
        result.Data.Owner.Should().NotBeNull();
        result.Message.Should().Be("Success");
        result.Data.Should().BeOfType<CreateBlogResponse>();
        
        result.Data.Id.ToString().Should().NotBeEmpty();
        result.Data.Name.Should().NotBeEmpty();
        result.Data.Description.Should().NotBeEmpty();
        result.Data.CreatedDate.ToString().Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task TestCreateBlog_CreateBlogWithInValidCommandShouldReturn_ValidationError()
    {
        var author = _authors[0];
        var authorID = author.Id;
        _authorReadRepository.Setup(x => x.GetByIdAsync(authorID,false)).ReturnsAsync(author);
        _createBlogCommandHandler = new CreateBlogCommandHandler(_blogBlogWriteRepository.Object, _authorReadRepository.Object, _mapper);
        
        var command = new CreateBlogCommand("", "", authorID);
        var validationResult = await _createBlogValidator.ValidateAsync(command);
        validationResult.Errors.Count.Should().Be(2);
    }

    [Theory]
    [InlineData("00ebf684-3797-473b-a503-a88c1c4cbb6d")]
    public async Task TestCreateBlog_CreateBlogWithInvalidAuthor_ShouldReturn_ErrorDataResult(string authorId)
    {
        
        _createBlogCommandHandler = new CreateBlogCommandHandler(_blogBlogWriteRepository.Object, _authorReadRepository.Object, _mapper);
        var command = new CreateBlogCommand("C#", "AspNet", Guid.Parse(authorId));
        var result = await _createBlogCommandHandler.Handle(command, CancellationToken.None);
        result.Message.Should().Be($"Author cannot found with id: {authorId}");
        result.Data.Should().BeNull();
    }
}