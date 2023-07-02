using FluentAssertions;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.DeleteAuthor;
using NUnit.Framework;

namespace MinimalApiCleanArchitecture.Application.IntegrationTests.Features.AuthorFeature.Commands.DeleteAuthor;
using static Testing;

public class DeleteAuthorCommandHandlerTests
{
    [Test]
    public async Task TestDeleteAuthor_DeleteAuthorShouldReturn_True()
    {
        var authorCommand = new CreateAuthorCommand("Jon", "Doe", "Developer", new DateTime(1990, 9, 1));
        var authorResult = await SendAsync(authorCommand);
        
        authorResult.Should().BeAssignableTo<CreateAuthorResponse>();

        var deleteAuthorCommand = new DeleteAuthorCommand(authorResult.Id);
        var deleteAuthorResult = await SendAsync(deleteAuthorCommand);

        deleteAuthorResult.Should().NotBeNull();
        deleteAuthorResult.Should().BeAssignableTo<DeleteAuthorResponse>();
        deleteAuthorResult.Status.Should().BeTrue();
    }

    [Test]
    public async Task TestDeleteAuthor_DeleteAuthorShouldReturn_NotFoundException()
    {
        var authorId = Guid.NewGuid();
        var deleteAuthorCommand = new DeleteAuthorCommand(authorId);
        await FluentActions.Invoking(() => SendAsync(deleteAuthorCommand)).Should().ThrowAsync<NotFoundException>();

    }
}