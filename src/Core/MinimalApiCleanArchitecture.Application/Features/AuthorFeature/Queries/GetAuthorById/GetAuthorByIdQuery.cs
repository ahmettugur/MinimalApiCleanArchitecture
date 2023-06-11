using MediatR;

namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAuthorById;

public class GetAuthorByIdQuery : IRequest<GetAuthorByIdResponse>
{
    public Guid AuthorId { get; private set; }
    public GetAuthorByIdQuery(Guid authorId)
    {
        AuthorId = authorId;
    }
}