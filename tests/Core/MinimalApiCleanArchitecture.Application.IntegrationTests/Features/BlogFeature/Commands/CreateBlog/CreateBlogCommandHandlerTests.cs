using FluentAssertions;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.CreateBlog;

namespace MinimalApiCleanArchitecture.Application.IntegrationTests.Features.BlogFeature.Commands.CreateBlog;

using static Testing;

public class CreateBlogCommandHandlerTests: BaseTestFixture
{
    [Fact]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateBlogCommand("","",Guid.NewGuid());

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }
}