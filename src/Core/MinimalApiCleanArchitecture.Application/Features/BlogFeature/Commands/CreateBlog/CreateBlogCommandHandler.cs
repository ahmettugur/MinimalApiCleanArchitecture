using AutoMapper;
using MediatR;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Authors;
using MinimalApiCleanArchitecture.Application.Interfaces.Repositories.Blogs;
using MinimalApiCleanArchitecture.Domain.Model;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.CreateBlog;

public class CreateBlogCommandHandler : IRequestHandler<CreateBlogCommand, IDataResult<CreateBlogResponse>>
{

    private readonly IBlogWriteRepository _blogWriteRepository;
    private readonly IAuthorReadRepository _authorReadRepository;
    private readonly IMapper _mapper;

    public CreateBlogCommandHandler(IBlogWriteRepository blogWriteRepository, IAuthorReadRepository authorReadRepository, IMapper mapper)
    {

        _blogWriteRepository = blogWriteRepository;
        _authorReadRepository = authorReadRepository;
        _mapper = mapper;
    }

    public async Task<IDataResult<CreateBlogResponse>> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
    {
        var blog = Blog.CreateBlog(request.Name, request.Description, request.AuthorId);
        var author = await _authorReadRepository.GetByIdAsync(blog.AuthorId);
        if (author is null)
        {
            return new ErrorDataResult<CreateBlogResponse>($"Author cannot found with id: {request.AuthorId}");
        }
        await _blogWriteRepository.AddAsync(blog);        
        await _blogWriteRepository.SaveChangesAsync();
        blog.SetOwner(author);
        var addedBlog = _mapper.Map<CreateBlogResponse>(blog);
        var result = new SuccessDataResult<CreateBlogResponse>(addedBlog, "Success");
        return result;
    }
}