using MediatR;
using MinimalApiCleanArchitecture.Application.Common.Results;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetAllBlogs;

public class GetAllBlogsQuery : IRequest<IDataResult<List<GetAllBlogsResponse>>>
{

}