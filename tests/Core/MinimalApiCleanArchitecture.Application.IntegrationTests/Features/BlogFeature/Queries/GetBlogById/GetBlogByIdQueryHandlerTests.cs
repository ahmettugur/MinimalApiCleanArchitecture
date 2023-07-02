using FluentAssertions;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.CreateBlog;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetBlogById;
using NUnit.Framework;

namespace MinimalApiCleanArchitecture.Application.IntegrationTests.Features.BlogFeature.Queries.GetBlogById;
using static Testing;

public class GetBlogByIdQueryHandlerTests: BaseTestFixture
{

    [Test]
    public async Task TestGetAllBlogById_GetAllBlogByIdShouldReturn_SuccessDataResult()
    {
        var authorCommand = new CreateAuthorCommand("Jon", "Doe", "Developer", new DateTime(1990, 9, 1));
        var authorResult = await SendAsync(authorCommand);

        authorResult.Should().BeAssignableTo<CreateAuthorResponse>();
        
        var blogCommand = new CreateBlogCommand("C¢QRS & Event Sourcing","Microservices",authorResult.Id);
        var blogResult = await SendAsync(blogCommand);
        
        blogResult.Data.Should().BeAssignableTo<CreateBlogResponse>();

        var query = new GetBlogByIdQuery(blogResult.Data.Id);
        var getBlogByIdQueryResult = await SendAsync(query);
        
        
        getBlogByIdQueryResult.Data.Owner.Should().NotBeNull();
        getBlogByIdQueryResult.Message.Should().Be("Success");
        getBlogByIdQueryResult.Data.Should().BeAssignableTo<GetBlogByIdResponse>();

    }
    
    [Test]
    public async Task TestGetAllBlogById_GetAllBlogByIdShouldReturn_ErrorDataResult()
    {
        var blogId = Guid.NewGuid();
        var query = new GetBlogByIdQuery(blogId);
        var getBlogByIdQueryResult = await SendAsync(query);
        

        getBlogByIdQueryResult.Should().BeAssignableTo<ErrorDataResult<GetBlogByIdResponse>>();
        getBlogByIdQueryResult.Message.Should().Be($"Blog cannot found with id: {blogId}");
        getBlogByIdQueryResult.Data.Should().BeNull();


    }
}