using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MinimalApiCleanArchitecture.Application.Common.Behaviours;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;
using MinimalApiCleanArchitecture.Domain.Model;
using Moq;
using Xunit;

namespace MinimalApiCleanArchitecture.Application.UnitTests.Features.AuthorFeature.Commands.CreateAuthor;

public class CreateAuthorCommandHandlerTests
{
    private CreateAuthorCommandHandler? _createAuthorCommandHandler;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<CreateAuthorCommand>> _logger;
    private readonly Mock<IAuthorWriteRepository> _authorWriteRepository;
    private readonly IServiceCollection _services;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<Author> _authors;
    private readonly CreateAuthorValidator _createAuthorValidator;
    

    public CreateAuthorCommandHandlerTests()
    {
        _authorWriteRepository = new Mock<IAuthorWriteRepository>();
        _services = new ServiceCollection();
        _services.AddApplicationServices();
        _services.AddLogging();

        _serviceProvider = _services.BuildServiceProvider();
        _mapper = _serviceProvider.GetService<IMapper>()!;

        _createAuthorValidator = new CreateAuthorValidator();
        
        _logger = new Mock<ILogger<CreateAuthorCommand>>();
        
        _authors = new List<Author>
        {
            new()
            {
                Id  = Guid.NewGuid() ,FirstName = "Jon",LastName = "Doe",Bio = "Developer",DateOfBirth = new DateTime(1990,9,1)
            }
        };
    }

    [Fact]
    public async Task TestCreateAuthor_CreateAuthorWithValidCommandShouldReturn_NoValidationException()
    {
        var author = _authors[0];

        var createAuthorRequest = new CreateAuthorRequest(author.FirstName!, author.LastName!, author.Bio!, author.DateOfBirth);

        var command = _mapper.Map<CreateAuthorCommand>(createAuthorRequest);
        
        var requestLogger = new LoggingBehaviour<CreateAuthorCommand>(_logger.Object);
        await requestLogger.Process(command, new CancellationToken());
        
        _createAuthorCommandHandler = new CreateAuthorCommandHandler(_authorWriteRepository.Object, _mapper);
        var validationBehaviour = new ValidationBehaviour<CreateAuthorCommand, CreateAuthorResponse>(new []{_createAuthorValidator});
        var requestHandlerDelegate = new RequestHandlerDelegate<CreateAuthorResponse>(() => _createAuthorCommandHandler.Handle(command, CancellationToken.None));
        var result = await validationBehaviour.Handle(command, requestHandlerDelegate, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.ToString().Should().NotBeEmpty();
        result.Name.Should().NotBeEmpty();
        result.Bio.Should().NotBeEmpty();
        result.DateOfBirth.Should().Be(author.DateOfBirth);
        result.Should().BeAssignableTo<CreateAuthorResponse>();

    }
    
    [Fact]
    public async Task TestCreateAuthor_CreateAuthorWithInValidCommandShouldReturn_ValidationException()
    {
        var author = _authors[0];

        var createAuthorRequest = new CreateAuthorRequest("", "", "", author.DateOfBirth);
        var command =  _mapper.Map<CreateAuthorCommand>(createAuthorRequest);
        _createAuthorCommandHandler = new CreateAuthorCommandHandler(_authorWriteRepository.Object, _mapper);
        var validationBehaviour = new ValidationBehaviour<CreateAuthorCommand, CreateAuthorResponse>(new []{_createAuthorValidator});

        Task<CreateAuthorResponse> requestHandlerDelegate() => _createAuthorCommandHandler.Handle(command, CancellationToken.None);
        requestHandlerDelegate().Should().NotBeNull();
        
        var requestHandlerDelegateResult = new RequestHandlerDelegate<CreateAuthorResponse>(requestHandlerDelegate);
        requestHandlerDelegateResult.Should().NotBeNull();
        
        Func<Task>  act = async () => await validationBehaviour.Handle(command, requestHandlerDelegateResult, CancellationToken.None);
        await act.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    public async Task TestCreateAuthor_CreateAuthorWithInValidCommandShouldReturn_UnhandledException()
    {
        var author = _authors[0];

        var createAuthorRequest = new CreateAuthorRequest(author.FirstName!, author.LastName!, author.Bio!, author.DateOfBirth);
        var command =  _mapper.Map<CreateAuthorCommand>(createAuthorRequest);
        _createAuthorCommandHandler = new CreateAuthorCommandHandler(_authorWriteRepository.Object, _mapper);
        var unhandledExceptionBehaviour = new UnhandledExceptionBehaviour<CreateAuthorCommand, CreateAuthorResponse>(_logger.Object);
        var requestHandlerDelegate = new RequestHandlerDelegate<CreateAuthorResponse>(() => _createAuthorCommandHandler.Handle(command, CancellationToken.None));
        var result = await unhandledExceptionBehaviour.Handle(command, requestHandlerDelegate, CancellationToken.None);
        result.Should().BeAssignableTo<CreateAuthorResponse>();
        command = null;
        await Assert.ThrowsAnyAsync<ArgumentException>( () => unhandledExceptionBehaviour.Handle(command, requestHandlerDelegate, CancellationToken.None));

    }
}