using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.DeleteAuthor;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;
using MinimalApiCleanArchitecture.Domain.Model;
using Moq;
using Xunit;

namespace MinimalApiCleanArchitecture.Application.UnitTests.Features.AuthorFeature.Commands.DeleteAuthor;

public class DeleteAuthorCommandHandlerTests
{
    private readonly IServiceCollection _services;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;
    private DeleteAuthorCommandHandler? _deleteAuthorCommandHandler;
    private readonly Mock<IAuthorWriteRepository> _authorWriteRepository;
    private readonly Mock<IAuthorReadRepository> _authorReadRepository;
    private readonly Author _author;

    public DeleteAuthorCommandHandlerTests()
    {
        _authorWriteRepository = new Mock<IAuthorWriteRepository>();
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
    public async Task TestDeleteAuthor_DeleteAuthorShouldReturn_True()
    {
        _authorReadRepository.Setup(_ => _.GetByIdAsync(_author.Id, false)).ReturnsAsync(_author);

        var deleteAuthorRequest = new DeleteAuthorRequest(_author.Id);
        var command = _mapper.Map<DeleteAuthorCommand>(deleteAuthorRequest);

        _deleteAuthorCommandHandler = new DeleteAuthorCommandHandler(_authorWriteRepository.Object, _authorReadRepository.Object);

        var result = await _deleteAuthorCommandHandler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<DeleteAuthorResponse>();
        result.Status.Should().BeTrue();
    }

    [Fact]
    public async Task TestDeleteAuthor_DeleteAuthorShouldReturn_NotFoundException()
    {
        var authorId = Guid.NewGuid();
        _authorReadRepository.Setup(_ => _.GetByIdAsync(_author.Id, false)).ReturnsAsync(_author);
        
        var deleteAuthorRequest = new DeleteAuthorRequest(authorId);
        var command = _mapper.Map<DeleteAuthorCommand>(deleteAuthorRequest);
        _deleteAuthorCommandHandler = new DeleteAuthorCommandHandler(_authorWriteRepository.Object, _authorReadRepository.Object);

        Func<Task> act = async () => await _deleteAuthorCommandHandler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Author cannot found with id: {authorId}");

    }
}