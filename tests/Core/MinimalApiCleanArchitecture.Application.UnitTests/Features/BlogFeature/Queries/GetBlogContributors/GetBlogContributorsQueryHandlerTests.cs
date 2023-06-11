using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetBlogContributors;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Blogs;
using MinimalApiCleanArchitecture.Domain.Model;
using Moq;
using Xunit;

namespace MinimalApiCleanArchitecture.Application.UnitTests.Features.BlogFeature.Queries.GetBlogContributors;

public class GetBlogContributorsQueryHandlerTests
{
    private readonly Mock<IBlogReadRepository> _blogReadRepository;
    private readonly List<Author> _authors;
    private readonly List<Blog> _blogs;
    private readonly IMapper _mapper;
    private readonly IServiceCollection _services;
    private readonly IServiceProvider _serviceProvider;
    
    private GetBlogContributorsQueryHandler? _getBlogContributorsQueryHandler;

    public GetBlogContributorsQueryHandlerTests()
    {
        _blogReadRepository = new Mock<IBlogReadRepository>();
        
        _services = new ServiceCollection();
        _services.AddApplicationServices();
        _services.AddLogging();

        _serviceProvider = _services.BuildServiceProvider();
        _mapper = _serviceProvider.GetService<IMapper>()!;
        
        var authorId = Guid.NewGuid();
        _authors = new List<Author>
        {
            new()
            {
                Id  = authorId ,FirstName = "Jon",LastName = "Doe",Bio = "Developer",DateOfBirth = new DateTime(1990,9,1)
            }
        };
        _blogs = new List<Blog>
        {
            Blog.CreateBlog("C#","C# Example",authorId)
        };
        
    }

    [Fact]
    public async Task TestGetBlogContributors_GetBlogContributorsShouldReturn_SuccessDataResult()
    { 
        _blogs[0].AddContributor(_authors[0]);
        var blog = _blogs[0];

        _blogReadRepository.Setup(x => x.GetByIdAsync(blog.Id, true, _ => _!.Contributors)).ReturnsAsync(blog);

        var query = new GetBlogContributorsQuery(blog.Id);

        _getBlogContributorsQueryHandler = new GetBlogContributorsQueryHandler(_blogReadRepository.Object, _mapper);
        var result = await _getBlogContributorsQueryHandler.Handle(query, CancellationToken.None);

        result.Data.Should().NotBeNull();
        result.Data.Count.Should().BeGreaterThan(0);
        result.Should().BeAssignableTo<SuccessDataResult<List<GetBlogContributorsResponse>>>();

        result.Data[0].ContributorId.Should().Be(blog.Contributors!.ToList()[0].Id);
        result.Data[0].Name.Should().Be(blog.Contributors!.ToList()[0].FullName);
        result.Data[0].Bio.Should().Be(blog.Contributors!.ToList()[0].Bio);

    }
    
    [Fact]
    public async Task TestGetBlogContributors_GetBlogContributorsShouldReturn_ErrorDataResult()
    {
        var blog = _blogs[0];
        _blogReadRepository.Setup(x => x.GetByIdAsync(blog.Id, true, _ => _!.Contributors)).ReturnsAsync(blog);

        blog.Id = Guid.NewGuid();
        var query = new GetBlogContributorsQuery(blog.Id);

        _getBlogContributorsQueryHandler = new GetBlogContributorsQueryHandler(_blogReadRepository.Object, _mapper);
        var result = await _getBlogContributorsQueryHandler.Handle(query, CancellationToken.None);

        result.Should().BeAssignableTo<ErrorDataResult<List<GetBlogContributorsResponse>>>();
        result.Data.Should().BeNull();
        result.Message.Should().Be($"Contributor cannot found with blog id: {blog.Id}");
        

    }
    
}