using FluentAssertions;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.AddContributor;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.CreateBlog;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetBlogContributors;
using NUnit.Framework;

namespace MinimalApiCleanArchitecture.Application.IntegrationTests.Features.BlogFeature.Queries.GetBlogContributors;

using static Testing;

public class GetBlogContributorsQueryHandlerTests: BaseTestFixture
{

    [Test]
    public async Task TestGetBlogContributors_GetBlogContributorsShouldReturn_SuccessDataResult()
    { 
       
        var authorCommand = new CreateAuthorCommand("Jon", "Doe", "Developer", new DateTime(1990, 9, 1));
        var authorResult = await SendAsync(authorCommand);

        authorResult.Should().BeAssignableTo<CreateAuthorResponse>();
        
        var blogCommand = new CreateBlogCommand("C¢QRS & Event Sourcing","Microservices",authorResult.Id);
        var blogResult = await SendAsync(blogCommand);
        
        blogResult.Data.Should().BeAssignableTo<CreateBlogResponse>();
        
        var addContributorCommand = new AddContributorCommand(blogResult.Data.Id, authorResult.Id);
        var addContributorCommandResult = await SendAsync(addContributorCommand);

        addContributorCommandResult.Should().BeAssignableTo<SuccessResult>();
        addContributorCommandResult.Success.Should().Be(true);
        
        var query = new GetBlogContributorsQuery(blogResult.Data.Id);
        var getBlogContributorsQueryResult = await SendAsync(query);
        
        getBlogContributorsQueryResult.Data.Should().NotBeNull();
        getBlogContributorsQueryResult.Data.Count.Should().BeGreaterThan(0);
        getBlogContributorsQueryResult.Should().BeAssignableTo<SuccessDataResult<List<GetBlogContributorsResponse>>>();
        
    }
    
    [Test]
    public async Task TestGetBlogContributors_GetBlogContributorsShouldReturn_ErrorDataResult()
    {
        var blogId = Guid.NewGuid();
        var query = new GetBlogContributorsQuery(blogId);
        var getBlogByIdQueryResult = await SendAsync(query);

        getBlogByIdQueryResult.Should().BeAssignableTo<ErrorDataResult<List<GetBlogContributorsResponse>>>();
        getBlogByIdQueryResult.Data.Should().BeNull();
        getBlogByIdQueryResult.Message.Should().Be($"Contributor cannot found with blog id: {blogId}");
    }
}