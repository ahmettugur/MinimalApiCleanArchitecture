using MediatR;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Blogs;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.AddContributor;


public class AddContributorCommandHandler : IRequestHandler<AddContributorCommand, IResult>
{

    private readonly IBlogReadRepository _blogReadRepository;
    private readonly IBlogWriteRepository _blogWriteRepository;
    private readonly IAuthorReadRepository _authorReadRepository;

    public AddContributorCommandHandler(IBlogReadRepository blogReadRepository, IBlogWriteRepository blogWriteRepository, IAuthorReadRepository authorReadRepository)
    {
        _blogReadRepository = blogReadRepository;
        _blogWriteRepository = blogWriteRepository;
        _authorReadRepository = authorReadRepository;
    }

    public async Task<IResult> Handle(AddContributorCommand request, CancellationToken cancellationToken)
    {
        var blog = await _blogReadRepository.GetByIdAsync(request.BlogId);
        if (blog is null) 
            throw new NotFoundException($"Blog cannot found with id: {request.BlogId}");       

        var author = await _authorReadRepository.GetByIdAsync(request.ContributorId);
        if (author is null)
            throw new NotFoundException($"Author cannot found with id: {request.ContributorId}");

        blog.AddContributor(author);
        await _blogWriteRepository.SaveChangesAsync();
        return new SuccessResult($"Contributor added successful with BlogId: {request.BlogId} and ContributorId: {request.ContributorId}" );
    }
}