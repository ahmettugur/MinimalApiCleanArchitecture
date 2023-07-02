using FluentAssertions;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.AddContributor;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.CreateBlog;
using NUnit.Framework;

namespace MinimalApiCleanArchitecture.Application.IntegrationTests.Features.BlogFeature.Commands.AddContributor;

using static Testing;

public class AddContributorCommandHandlerTests
{
    
    [Test]
    public async Task TestAddContributor_AddContributorShouldReturn_SuccessDataResult()
    {

        var authorCommand = new CreateAuthorCommand("Jon", "Doe", "Developer", new DateTime(1990, 9, 1));
        var authorResult = await SendAsync(authorCommand);
        authorResult.Should().BeAssignableTo<CreateAuthorResponse>();
        
        var blogCommand = new CreateBlogCommand("C¢QRS & Event Sourcing","Microservices",authorResult.Id);
        var blogResult = await SendAsync(blogCommand);

        blogResult.Should().BeAssignableTo<SuccessDataResult<CreateBlogResponse>>();

        var addContributorCommand = new AddContributorCommand(blogResult.Data.Id, authorResult.Id);
        var addContributorCommandResult = await SendAsync(addContributorCommand);

        addContributorCommandResult.Should().BeAssignableTo<SuccessResult>();
        addContributorCommandResult.Success.Should().Be(true);
        
        
        blogResult.Data.Owner!.FirstName.Should().Be("Jon");
        blogResult.Data.Owner!.LastName.Should().Be("Doe");
        blogResult.Data.Owner!.FullName.Should().Be("Jon Doe");
        blogResult.Data.Owner!.Bio.Should().Be("Developer");
        blogResult.Data.Owner!.DateOfBirth.Should().Be(new DateTime(1990, 9, 1));

    }
    
    [Test]
    public async Task TestAddContributor_AddContributorNoneExistentAuthorShouldReturn_ThrowNotfoundException()
    {
        var authorCommand = new CreateAuthorCommand("Jon", "Doe", "Developer", new DateTime(1990, 9, 1));
        var authorResult = await SendAsync(authorCommand);
        authorResult.Should().BeAssignableTo<CreateAuthorResponse>();
        
        var blogCommand = new CreateBlogCommand("C¢QRS & Event Sourcing","Microservices",authorResult.Id);
        var blogResult = await SendAsync(blogCommand);

        var command = new AddContributorCommand(blogResult.Data.Id, Guid.NewGuid());
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task TestAddContributor_AddContributorNoneExistentBlogShouldReturn_ThrowNotfoundException()
    {
        var authorCommand = new CreateAuthorCommand("Jon", "Doe", "Developer", new DateTime(1990, 9, 1));
        var authorResult = await SendAsync(authorCommand);
        authorResult.Should().BeAssignableTo<CreateAuthorResponse>();
        
        var command = new AddContributorCommand(Guid.NewGuid(), authorResult.Id);
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }
}