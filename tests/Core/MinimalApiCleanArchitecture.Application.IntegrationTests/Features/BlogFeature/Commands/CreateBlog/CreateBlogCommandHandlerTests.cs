using FluentAssertions;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.CreateBlog;

namespace MinimalApiCleanArchitecture.Application.IntegrationTests.Features.BlogFeature.Commands.CreateBlog;

using static Testing;

public class CreateBlogCommandHandlerTests: BaseTestFixture
{
    [Fact]
    public async Task TestCreateBlog_CreateBlogWithInValidCommandShouldReturn_ValidationError()
    {
        var command = new CreateBlogCommand("","",Guid.NewGuid());

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task TestCreateBlog_CreateBlogWithInvalidAuthor_ShouldReturn_ErrorDataResult()
    {
        var command = new CreateBlogCommand("C¢QRS & Event Sourcing","Microservices",Guid.NewGuid());

        var result = await SendAsync(command);
        result.Should().BeAssignableTo<ErrorDataResult<CreateBlogResponse>>();
        result.Message.Should().Be($"Author cannot found with id: {command.AuthorId}");
        result.Data.Should().BeNull();
    }

    
    [Fact]
    public async Task TestCreateBlog_CreateBlogWithValidCommandShouldReturn_SuccessDataResult()
    {
        var authorCommand = new CreateAuthorCommand("Jon", "Doe", "Developer", new DateTime(1990, 9, 1));
        var authorResult = await SendAsync(authorCommand);

        authorResult.Should().BeAssignableTo<CreateAuthorResponse>();
        
        var blogCommand = new CreateBlogCommand("C¢QRS & Event Sourcing","Microservices",authorResult.Id);
        var blogResult = await SendAsync(blogCommand);
        
        blogResult.Data.Owner.Should().NotBeNull();
        blogResult.Message.Should().Be("Success");
        blogResult.Data.Should().BeOfType<CreateBlogResponse>();
    }
}