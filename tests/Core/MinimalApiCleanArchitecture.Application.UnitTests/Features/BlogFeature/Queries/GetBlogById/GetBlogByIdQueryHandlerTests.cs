using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetBlogById;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Blogs;
using MinimalApiCleanArchitecture.Domain.Model;
using Moq;
using Xunit;

namespace MinimalApiCleanArchitecture.Application.UnitTests.Features.BlogFeature.Queries.GetBlogById;

public class GetBlogByIdQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IBlogReadRepository> _blogReadRepository;
    private readonly IServiceCollection _services;
    private readonly IServiceProvider _serviceProvider;
    private GetBlogByIdQueryHandler? _getBlogByIdQueryHandler;
    private readonly List<Blog> _blogs;

    
    public GetBlogByIdQueryHandlerTests()
    {
        _blogReadRepository = new Mock<IBlogReadRepository>();
        _services = new ServiceCollection();
        _services.AddApplicationServices();
        _services.AddLogging();

        _serviceProvider = _services.BuildServiceProvider();
        _mapper = _serviceProvider.GetService<IMapper>()!;
        
        _blogs = new List<Blog>
        {
            Blog.CreateBlog("C#","C# Example",Guid.NewGuid())
        };
    }

    [Fact]
    public async Task TestGetAllBlogById_GetAllBlogByIdShouldReturn_SuccessDataResult()
    {
        var blog = _blogs[0];

        _blogReadRepository.Setup(_ => _.GetByIdAsync(blog.Id,true,_ => _!.Owner)).ReturnsAsync(blog);

        _getBlogByIdQueryHandler = new GetBlogByIdQueryHandler(_blogReadRepository.Object, _mapper);

        var query = new GetBlogByIdQuery(blog.Id);

        var result = await _getBlogByIdQueryHandler.Handle(query, CancellationToken.None);

        result.Data.Should().NotBeNull();
        result.Message.Should().Be("Success");
        result.Data.Owner.Should().BeNull();
        result.Should().BeAssignableTo<DataResult<GetBlogByIdResponse>>();

                
        result.Data.Id.ToString().Should().NotBeEmpty();
        result.Data.Name.Should().NotBeEmpty();
        result.Data.Description.Should().NotBeEmpty();
        result.Data.CreatedDate.ToString().Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task TestGetAllBlogById_GetAllBlogByIdShouldReturn_ErrorDataResult()
    {
        Blog? blog = null;
        var blogId = Guid.NewGuid();
        _blogReadRepository.Setup(_ => _.GetByIdAsync(blogId, true, _ => _!.Owner)).ReturnsAsync(blog);

        var query = new GetBlogByIdQuery(blogId);

        _getBlogByIdQueryHandler = new GetBlogByIdQueryHandler(_blogReadRepository.Object, _mapper);

        var result = await _getBlogByIdQueryHandler.Handle(query, CancellationToken.None);

        result.Should().BeAssignableTo<ErrorDataResult<GetBlogByIdResponse>>();
        result.Message.Should().Be($"Blog cannot found with id: {blogId}");
        result.Data.Should().BeNull();


    }
}