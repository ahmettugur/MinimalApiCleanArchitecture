using FluentAssertions;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAllAuthors;
using NUnit.Framework;

namespace MinimalApiCleanArchitecture.Application.IntegrationTests.Features.AuthorFeature.Queries.GetAllAuthors;

using static Testing;

public class GetAllAuthorsQueryHandlerTests
{

    [Test]
    public async Task TestGetAllAuthors_GetAllAuthorsShouldReturn_GetAllAuthors()
    {
        var query = new GetAllAuthorsQuery();
        var result = await SendAsync(query);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<List<GetAllAuthorsResponse>>();
        
    }
}