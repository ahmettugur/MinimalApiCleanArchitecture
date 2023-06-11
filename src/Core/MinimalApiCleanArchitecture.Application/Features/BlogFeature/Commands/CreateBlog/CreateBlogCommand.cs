using MediatR;
using MinimalApiCleanArchitecture.Application.Common.Results;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.CreateBlog;

public class CreateBlogCommand : IRequest<IDataResult<CreateBlogResponse>>
{
    public string Name { get; private set; }
    public string Description { get; private set; } 
    public Guid AuthorId { get; private set; }

    public CreateBlogCommand(string name, string description, Guid authorId)
    {
        Name = name;
        Description = description;
        AuthorId = authorId;
    }
}
