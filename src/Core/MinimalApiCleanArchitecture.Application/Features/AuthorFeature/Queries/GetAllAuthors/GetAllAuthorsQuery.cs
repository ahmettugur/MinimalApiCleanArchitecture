using MediatR;

namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAllAuthors;

public class GetAllAuthorsQuery : IRequest<List<GetAllAuthorsResponse>>
{

}