using FluentAssertions;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.UpdateAuthor;
using NUnit.Framework;

namespace MinimalApiCleanArchitecture.Application.IntegrationTests.Features.AuthorFeature.Commands.UpdateAuthor;

using static Testing;

public class UpdateAuthorCommandHandlerTest
{
   
    [Test]
    public async Task TestUpdateAuthor_UpdateAuthorWithValidCommandShouldReturn_UpdateAuthorResponse()
    {
        var createAuthorCommand = new CreateAuthorCommand("Jon", "Doe", "Developer", new DateTime(1990, 9, 1));
        var createAuthorResult = await SendAsync(createAuthorCommand);
        
        createAuthorResult.Should().BeAssignableTo<CreateAuthorResponse>();

        var updateAuthorCommand = new UpdateAuthorCommand(createAuthorResult.Id, createAuthorCommand.FirstName, createAuthorCommand.LastName, "Software Developer", createAuthorCommand.DateOfBirth);
        var updateAuthorResult = await SendAsync(updateAuthorCommand);
        
        updateAuthorResult.Should().NotBeNull();
        updateAuthorResult.Should().BeAssignableTo<UpdateAuthorResponse>();
        updateAuthorResult.Status.Should().Be(true);
        
    }
    
    [Test]
    public async Task TestUpdateAuthor_UpdateAuthorWithInvalidValidCommandShouldReturn_ValidationException()
    {
        var createAuthorCommand = new CreateAuthorCommand("Jon", "Doe", "Developer", new DateTime(1990, 9, 1));
        var createAuthorResult = await SendAsync(createAuthorCommand);
        
        createAuthorResult.Should().BeAssignableTo<CreateAuthorResponse>();

        var updateAuthorCommand = new UpdateAuthorCommand(createAuthorResult.Id, "", "", "", createAuthorCommand.DateOfBirth);
        await FluentActions.Invoking(() => SendAsync(updateAuthorCommand)).Should().ThrowAsync<ValidationException>();
    }
    
    [Test]
    public async Task TestUpdateAuthor_UpdateAuthorWithInvalidAuthorShouldReturn_NotFoundException()
    {
        var updateAuthorCommand = new UpdateAuthorCommand(Guid.NewGuid(), "Jon", "Doe", "Developer", new DateTime(1990, 9, 1));
        await FluentActions.Invoking(() => SendAsync(updateAuthorCommand)).Should().ThrowAsync<NotFoundException>();
    }
}