using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAuthorById;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;
using MinimalApiCleanArchitecture.Domain.Model;
using Moq;
using Xunit;

namespace MinimalApiCleanArchitecture.Application.UnitTests.Features.AuthorFeature.Queries.GetAuthorById;

public class GetAuthorByIdQueryHandlerTests
{
    private readonly Mock<IAuthorReadRepository> _authorReadRepository;
    private readonly IMapper _mapper;
    private readonly IServiceCollection _services;
    private readonly IServiceProvider _serviceProvider;
    private GetAuthorByIdQueryHandler? _getAuthorByIdQueryHandler;
    private readonly Author _author;

    public GetAuthorByIdQueryHandlerTests()
    {
        _authorReadRepository = new Mock<IAuthorReadRepository>();
        _services = new ServiceCollection();
        _services.AddApplicationServices();

        _serviceProvider = _services.BuildServiceProvider();
        _mapper = _serviceProvider.GetService<IMapper>()!;

        _author = new Author
        {
            Id = Guid.NewGuid(), FirstName = "Jon", LastName = "Doe", Bio = "Developer",
            DateOfBirth = new DateTime(1990, 9, 1)
        };

    }

    [Fact]
    public async Task TestGetAuthorById_GetAuthorByIdShouldReturn_GetAuthor()
    {
        
        _authorReadRepository.Setup(_ => _.GetByIdAsync(_author.Id, false)).ReturnsAsync(_author);
        
        _getAuthorByIdQueryHandler = new GetAuthorByIdQueryHandler(_authorReadRepository.Object, _mapper);
        var result = await _getAuthorByIdQueryHandler.Handle(new GetAuthorByIdQuery(_author.Id),CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<GetAuthorByIdResponse>();

        result.Id.Should().Be(_author.Id);
        result.Name.Should().Be(_author.FullName);
        result.Bio.Should().Be(_author.Bio);
        result.DateOfBirth.Should().Be(_author.DateOfBirth);

    }

    [Fact]
    public async Task TestGetAuthorById_GetAuthorByIdShouldReturn_NotFoundException()
    {
        var authorId = Guid.NewGuid();
        _authorReadRepository.Setup(_ => _.GetByIdAsync(_author.Id, false)).ReturnsAsync(_author);
        
        _getAuthorByIdQueryHandler = new GetAuthorByIdQueryHandler(_authorReadRepository.Object, _mapper);

        Func<Task>  act = async () => await _getAuthorByIdQueryHandler.Handle(new GetAuthorByIdQuery(authorId), CancellationToken.None);

       await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Author cannot found with id: {authorId}");
    }
}