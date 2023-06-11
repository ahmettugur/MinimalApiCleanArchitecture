using MediatR;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Blogs;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.RemoveContributor;

public class RemoveContributorCommandHandler : IRequestHandler<RemoveContributorCommand, IResult>
{
    private readonly IBlogReadRepository _blogReadRepository;
    private readonly IBlogWriteRepository _blogWriteRepository;
    private readonly IAuthorReadRepository _authorReadRepository;

    public RemoveContributorCommandHandler(IBlogReadRepository blogReadRepository, IBlogWriteRepository blogWriteRepository, IAuthorReadRepository authorReadRepository)
    {
        _blogReadRepository = blogReadRepository;
        _blogWriteRepository = blogWriteRepository;
        _authorReadRepository = authorReadRepository;
    }

    public async Task<IResult> Handle(RemoveContributorCommand request, CancellationToken cancellationToken)
    {
        var blog = await _blogReadRepository.GetByIdAsync(request.BlogId, false,_ => _!.Contributors);
        if (blog is null)
            throw new NotFoundException($"Blog cannot found with id: {request.BlogId}");
        
        var author = await _authorReadRepository.GetByIdAsync(request.ContributorId);
        if (author is null)
            throw new NotFoundException($"Author cannot found with id: {request.ContributorId}");
   
        blog.RemoveContributor(author);
        await _blogWriteRepository.SaveChangesAsync();
        return new SuccessResult($"Contributor removed successful with ContributorId: {request.ContributorId} for BlogId: {request.BlogId} " );
    }
}