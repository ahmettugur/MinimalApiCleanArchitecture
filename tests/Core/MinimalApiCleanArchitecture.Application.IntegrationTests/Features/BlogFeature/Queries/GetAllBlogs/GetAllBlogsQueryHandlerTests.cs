using FluentAssertions;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetAllBlogs;

namespace MinimalApiCleanArchitecture.Application.IntegrationTests.Features.BlogFeature.Queries.GetAllBlogs;

using static Testing;

public class GetAllBlogsQueryHandlerTests: BaseTestFixture
{

    [Fact]
    public async Task TestGetAllBlogs_GetAllBlogsShouldReturn_SuccessDataResult()
    {
        var query = new GetAllBlogsQuery();

        var result = await SendAsync(query);

        result.Should().BeAssignableTo<SuccessDataResult<List<GetAllBlogsResponse>>>();
        result.Data.Count.Should().BeGreaterThanOrEqualTo(0);
    }
    
    
}