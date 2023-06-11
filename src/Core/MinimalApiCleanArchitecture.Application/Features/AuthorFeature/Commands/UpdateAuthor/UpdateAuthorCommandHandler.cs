using MediatR;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;

namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.UpdateAuthor;

public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, UpdateAuthorResponse>
{

    private readonly IAuthorWriteRepository _authorWriteRepository;
    private readonly IAuthorReadRepository _authorReadRepository;

    public UpdateAuthorCommandHandler(IAuthorWriteRepository authorWriteRepository, IAuthorReadRepository authorReadRepository)
    {
        _authorWriteRepository = authorWriteRepository;
        _authorReadRepository = authorReadRepository;
    }

    public async Task<UpdateAuthorResponse> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await _authorReadRepository.GetByIdAsync(request.AuthorId);
        if (author is null)
            throw new NotFoundException($"Author cannot found with id: {request.AuthorId}");
        
        author.UpdateAuthor(request.FirstName, request.LastName, request.Bio, request.DateOfBirth);
        await _authorWriteRepository.SaveChangesAsync();
        var result = new UpdateAuthorResponse
        {
            Status = true
        };
        return result;
    }
}