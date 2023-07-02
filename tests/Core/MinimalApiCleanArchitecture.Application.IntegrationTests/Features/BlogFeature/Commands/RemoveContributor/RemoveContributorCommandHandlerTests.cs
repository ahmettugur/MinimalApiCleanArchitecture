using FluentAssertions;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.AddContributor;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.CreateBlog;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.RemoveContributor;
using NUnit.Framework;

namespace MinimalApiCleanArchitecture.Application.IntegrationTests.Features.BlogFeature.Commands.RemoveContributor;

using static Testing;

public class RemoveContributorCommandHandlerTests: BaseTestFixture
{
    [Test]
    public async Task TestRemoveContributor_RemoveContributorShouldReturn_NoException()
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
        addContributorCommandResult.Success.Should().BeTrue();
        
        
        var removeContributorCommand = new RemoveContributorCommand(blogResult.Data.Id, authorResult.Id);
        var removeContributorCommandResult = await SendAsync(removeContributorCommand);

        removeContributorCommandResult.Should().BeAssignableTo<SuccessResult>();
        removeContributorCommandResult.Success.Should().BeTrue();
    }
    
    
    [Test]
    public async Task TestRemoveContributor_RemoveContributorShouldReturn_AuthorNotFoundException()
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
        addContributorCommandResult.Success.Should().BeTrue();
        
        var removeContributorCommand = new RemoveContributorCommand(blogResult.Data.Id, Guid.NewGuid());
        await FluentActions.Invoking(() => SendAsync(removeContributorCommand)).Should().ThrowAsync<NotFoundException>();
    }
    
    [Test]
    public async Task TestAddAndRemoveContributor_RemoveContributorShouldReturn_BlogNotFoundException()
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
        addContributorCommandResult.Success.Should().BeTrue();
        
        var removeContributorCommand = new RemoveContributorCommand(Guid.NewGuid(), authorResult.Id);
        await FluentActions.Invoking(() => SendAsync(removeContributorCommand)).Should().ThrowAsync<NotFoundException>();
    }
}