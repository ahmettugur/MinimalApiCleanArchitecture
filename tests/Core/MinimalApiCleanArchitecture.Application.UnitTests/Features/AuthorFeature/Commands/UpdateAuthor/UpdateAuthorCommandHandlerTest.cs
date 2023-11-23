using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.Application.Common.Behaviours;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.UpdateAuthor;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;
using MinimalApiCleanArchitecture.Domain.Model;
using Moq;
using Xunit;

namespace MinimalApiCleanArchitecture.Application.UnitTests.Features.AuthorFeature.Commands.UpdateAuthor;

public class UpdateAuthorCommandHandlerTest
{
    private UpdateAuthorCommandHandler? _updateAuthorCommandHandler;
    private readonly IMapper _mapper;
    private readonly Mock<IAuthorWriteRepository> _authorWriteRepository;
    private readonly Mock<IAuthorReadRepository> _authorReadRepository;
    private readonly IServiceCollection _services;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<Author> _authors;
    private readonly UpdateAuthorValidator _updateAuthorValidator;

    public UpdateAuthorCommandHandlerTest()
    {
        _authorWriteRepository = new Mock<IAuthorWriteRepository>();
        _authorReadRepository = new Mock<IAuthorReadRepository>();
        _services = new ServiceCollection();
        _services.AddApplicationServices();

        _serviceProvider = _services.BuildServiceProvider();
        _mapper = _serviceProvider.GetService<IMapper>()!;

        _updateAuthorValidator = new UpdateAuthorValidator();
        
        _authors = new List<Author>
        {
            new()
            {
                Id  = Guid.NewGuid() ,FirstName = "Jon",LastName = "Doe",Bio = "Developer",DateOfBirth = new DateTime(1990, 9, 1,0,0,0,DateTimeKind.Utc)
            }
        };
    }
    
    [Fact]
    public async Task TestUpdateAuthor_UpdateAuthorWithValidCommandShouldReturn_NoValidationException()
    {
        var author = _authors[0];

        _authorReadRepository.Setup(_ => _.GetByIdAsync(author.Id,false)).ReturnsAsync(author);
        
        var updateAuthorRequest = new UpdateAuthorRequest(author.Id,author.FirstName!, author.LastName!, author.Bio!, author.DateOfBirth);

        var command = _mapper.Map<UpdateAuthorCommand>(updateAuthorRequest);
        _updateAuthorCommandHandler = new UpdateAuthorCommandHandler(_authorWriteRepository.Object,_authorReadRepository.Object);
        
        var validationBehaviour = new ValidationBehaviour<UpdateAuthorCommand, UpdateAuthorResponse>(new []{_updateAuthorValidator});
        var requestHandlerDelegate = new RequestHandlerDelegate<UpdateAuthorResponse>(() => _updateAuthorCommandHandler.Handle(command, CancellationToken.None));

        var result = await validationBehaviour.Handle(command, requestHandlerDelegate, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<UpdateAuthorResponse>();
        result.Status.Should().Be(true);
        
    }
    
    [Fact]
    public async Task TestUpdateAuthor_UpdateAuthorWithInvalidValidCommandShouldReturn_ValidationException()
    {
        var author = _authors[0];

        _authorReadRepository.Setup(_ => _.GetByIdAsync(author.Id,false)).ReturnsAsync(author);
        
        var updateAuthorRequest = new UpdateAuthorRequest(author.Id,"", "", author.Bio!, author.DateOfBirth);
        var command = _mapper.Map<UpdateAuthorCommand>(updateAuthorRequest);
       
       _updateAuthorCommandHandler = new UpdateAuthorCommandHandler(_authorWriteRepository.Object, _authorReadRepository.Object);    

        var validationBehaviour = new ValidationBehaviour<UpdateAuthorCommand, UpdateAuthorResponse>(new []{_updateAuthorValidator});

        Task<UpdateAuthorResponse> requestHandlerDelegate() => _updateAuthorCommandHandler.Handle(command, CancellationToken.None);
        requestHandlerDelegate().Should().NotBeNull();
        
        var requestHandlerDelegateResult = new RequestHandlerDelegate<UpdateAuthorResponse>(requestHandlerDelegate);
        
        await Assert.ThrowsAnyAsync<ValidationException>( () => validationBehaviour.Handle(command, requestHandlerDelegateResult, CancellationToken.None));

    }
    
    [Fact]
    public async Task TestUpdateAuthor_UpdateAuthorWithInvalidAuthorShouldReturn_NotFoundException()
    {
        var author = _authors[0];

        _authorReadRepository.Setup(_ => _.GetByIdAsync(author.Id,false)).ReturnsAsync(author);
        
        var command = new UpdateAuthorCommand(Guid.NewGuid(), author.FirstName!, author.LastName!, author.Bio!, author.DateOfBirth);
        
        _updateAuthorCommandHandler = new UpdateAuthorCommandHandler(_authorWriteRepository.Object,_authorReadRepository.Object);
        
        await Assert.ThrowsAnyAsync<NotFoundException>( () => _updateAuthorCommandHandler.Handle(command, CancellationToken.None));
    }
}