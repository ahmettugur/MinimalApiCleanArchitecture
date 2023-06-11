using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetAllBlogs;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Blogs;
using MinimalApiCleanArchitecture.Domain.Model;
using Moq;
using Xunit;

namespace MinimalApiCleanArchitecture.Application.UnitTests.Features.BlogFeature.Queries.GetAllBlogs;

public class GetAllBlogsQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IBlogReadRepository> _blogReadRepository;
    private readonly IServiceCollection _services;
    private readonly IServiceProvider _serviceProvider;
    private GetAllBlogsQueryHandler? _getAllBlogsQueryHandler;
    private readonly ILogger<GetAllBlogsQueryHandler> _logger;
    private readonly List<Blog> _blogs;

    public GetAllBlogsQueryHandlerTests()
    {
        _blogReadRepository = new Mock<IBlogReadRepository>();
        _services = new ServiceCollection();
        _services.AddApplicationServices();
        _services.AddLogging();

        _serviceProvider = _services.BuildServiceProvider();
        _mapper = _serviceProvider.GetService<IMapper>()!;
        
        var factory = _serviceProvider.GetService<ILoggerFactory>()!;
        _logger = factory.CreateLogger<GetAllBlogsQueryHandler>();
        
        _blogs = new List<Blog>
        {
            Blog.CreateBlog("C#","C# Example",Guid.NewGuid())
        };
    }

    [Fact]
    public async Task TestGetAllBlogs_GetAllBlogsShouldReturn_SuccessDataResult()
    {
        _blogReadRepository.Setup(_ => _.Get(true,null, null, _ => _!.Owner))!.ReturnsAsync(_blogs);

        var query = new GetAllBlogsQuery();

        _getAllBlogsQueryHandler = new GetAllBlogsQueryHandler(_blogReadRepository.Object, _mapper, _logger);

        var result = await _getAllBlogsQueryHandler.Handle(query, CancellationToken.None);

        result.Data.Count.Should().BeGreaterThan(0);
        result.Message.Should().Be("Success");
        result.Data[0].Owner.Should().BeNull();
        result.Should().BeAssignableTo<SuccessDataResult<List<GetAllBlogsResponse>>>();
        
        result.Should().NotBeNull();
        result.Data[0].Id.ToString().Should().NotBeEmpty();
        result.Data[0].Name.Should().NotBeEmpty();
        result.Data[0].Description.Should().NotBeEmpty();
        result.Data[0].CreatedDate.ToString().Should().NotBeEmpty();
    }
    
    
}