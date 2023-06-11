using MediatR;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;

namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.DeleteAuthor;

public class DeleteAuthorCommandHandler : IRequestHandler<DeleteAuthorCommand, DeleteAuthorResponse>
{
    private readonly IAuthorWriteRepository _authorWriteRepository;
    private readonly IAuthorReadRepository _authorReadRepository;

    public DeleteAuthorCommandHandler(IAuthorWriteRepository authorWriteRepository, IAuthorReadRepository authorReadRepository)
    {
        _authorWriteRepository = authorWriteRepository;
        _authorReadRepository = authorReadRepository;
    }
    public async Task<DeleteAuthorResponse> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await _authorReadRepository.GetByIdAsync(request.AuthorId);
        if (author is null)
            throw new NotFoundException($"Author cannot found with id: {request.AuthorId}");
        
        _authorWriteRepository.Remove(author);
        await _authorWriteRepository.SaveChangesAsync();
        var result = new DeleteAuthorResponse
        {
            Status = true
        };
        return result;
    }
}