using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MinimalApiCleanArchitecture.Application.Common.Behaviours;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAllAuthors;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;
using MinimalApiCleanArchitecture.Domain.Model;
using Moq;
using Xunit;

namespace MinimalApiCleanArchitecture.Application.UnitTests.Features.AuthorFeature.Queries.GetAllAuthors;

public class GetAllAuthorsQueryHandlerTests
{
    private readonly Mock<IAuthorReadRepository> _authorReadRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllAuthorsQuery> _logger;
    private readonly IServiceCollection _services;
    private readonly IServiceProvider _serviceProvider;
    private GetAllAuthorsQueryHandler? _getAllAuthorsQueryHandler;
    private readonly List<Author> _authors;

    public GetAllAuthorsQueryHandlerTests()
    {
        _authorReadRepository = new Mock<IAuthorReadRepository>();
        _services = new ServiceCollection();
        _services.AddApplicationServices();
        _services.AddLogging();

        _serviceProvider = _services.BuildServiceProvider();
        _mapper = _serviceProvider.GetService<IMapper>()!;
        
        var factory = _serviceProvider.GetService<ILoggerFactory>()!;
        _logger = factory.CreateLogger<GetAllAuthorsQuery>();
        
        _authors = new List<Author>
        {
            new()
            {
                Id  = Guid.NewGuid() ,FirstName = "Jon",LastName = "Doe",Bio = "Developer",DateOfBirth = new DateTime(1990,9,1)
            }
        };
    }

    [Fact]
    public async Task TestGetAllAuthors_GetAllAuthorsWithPerformanceBehaviourShouldReturn_GetAllAuthors()
    {
        var query = new GetAllAuthorsQuery();
        _authorReadRepository.Setup(x => x.GetAll(false))!.ReturnsAsync(_authors);

        _getAllAuthorsQueryHandler = new GetAllAuthorsQueryHandler(_authorReadRepository.Object, _mapper);
        
        var performanceBehaviour = new PerformanceBehaviour<GetAllAuthorsQuery,List<GetAllAuthorsResponse>>(_logger);
        
        var requestHandlerDelegate = new RequestHandlerDelegate<List<GetAllAuthorsResponse>>(() =>
        {
            Task.Delay(500);
            return _getAllAuthorsQueryHandler.Handle(query, CancellationToken.None);
        });

        var result = await performanceBehaviour.Handle(query, requestHandlerDelegate, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<List<GetAllAuthorsResponse>>();
    }
    
    [Fact]
    public async Task TestGetAllAuthors_GetAllAuthorsShouldReturn_GetAllAuthors()
    {
        var query = new GetAllAuthorsQuery();
        _authorReadRepository.Setup(x => x.GetAll(false))!.ReturnsAsync(_authors);

        _getAllAuthorsQueryHandler = new GetAllAuthorsQueryHandler(_authorReadRepository.Object, _mapper);
        
        var result = await _getAllAuthorsQueryHandler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<List<GetAllAuthorsResponse>>();
        
        result[0].Id.Should().Be(_authors[0].Id);
        result[0].Name.Should().Be(_authors[0].FullName);
        result[0].Bio.Should().Be(_authors[0].Bio);
        result[0].DateOfBirth.Should().Be(_authors[0].DateOfBirth);
    }
}