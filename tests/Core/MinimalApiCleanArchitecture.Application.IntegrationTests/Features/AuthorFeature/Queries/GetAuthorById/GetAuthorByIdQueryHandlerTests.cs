using FluentAssertions;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAuthorById;
using NUnit.Framework;

namespace MinimalApiCleanArchitecture.Application.IntegrationTests.Features.AuthorFeature.Queries.GetAuthorById;
using static Testing;

public class GetAuthorByIdQueryHandlerTests
{
    [Test]
    public async Task TestGetAuthorById_GetAuthorByIdShouldReturn_GetAuthor()
    {
        var authorCommand = new CreateAuthorCommand("Jon", "Doe", "Developer", new DateTime(1990, 9, 1));
        var authorResult = await SendAsync(authorCommand);
        
        authorResult.Should().BeAssignableTo<CreateAuthorResponse>();

        var query = new GetAuthorByIdQuery(authorResult.Id);
        var result = await SendAsync(query);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<GetAuthorByIdResponse>();
    }

    [Test]
    public async Task TestGetAuthorById_GetAuthorByIdShouldReturn_NotFoundException()
    {
        var authorId = Guid.NewGuid();
        var query = new GetAuthorByIdQuery(authorId);
        await FluentActions.Invoking(() => SendAsync(query)).Should().ThrowAsync<NotFoundException>();
    }
}