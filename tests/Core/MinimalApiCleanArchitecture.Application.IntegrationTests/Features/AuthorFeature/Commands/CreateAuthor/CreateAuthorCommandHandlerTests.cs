using FluentAssertions;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using NUnit.Framework;

namespace MinimalApiCleanArchitecture.Application.IntegrationTests.Features.AuthorFeature.Commands.CreateAuthor;

using static Testing;

public class CreateAuthorCommandHandlerTests
{
    [Test]
    public async Task TestCreateAuthor_CreateAuthorWithValidCommandShouldReturn_NoValidationException()
    {
        var authorCommand = new CreateAuthorCommand("Jon", "Doe", "Developer", new DateTime(1990, 9, 1));
        var authorResult = await SendAsync(authorCommand);
        
        authorResult.Should().BeAssignableTo<CreateAuthorResponse>();
    }
    
    [Test]
    public async Task TestCreateAuthor_CreateAuthorWithInValidCommandShouldReturn_ValidationException()
    {
        var authorCommand = new CreateAuthorCommand("", "", "Developer", new DateTime(1990, 9, 1));
        await FluentActions.Invoking(() => SendAsync(authorCommand)).Should().ThrowAsync<ValidationException>();
    }
    
    [Test]
    public async Task TestCreateAuthor_CreateAuthorWithInValidCommandShouldReturn_UnhandledException()
    {
        CreateAuthorCommand? authorCommand = null;
        await FluentActions.Invoking(() => SendAsync(authorCommand)).Should().ThrowAsync<Exception>();
    }
}