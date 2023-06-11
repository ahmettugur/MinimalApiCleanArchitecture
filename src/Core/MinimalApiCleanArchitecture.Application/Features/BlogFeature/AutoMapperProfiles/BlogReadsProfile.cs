using AutoMapper;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetAllBlogs;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetBlogById;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Queries.GetBlogContributors;
using MinimalApiCleanArchitecture.Domain.Model;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.AutoMapperProfiles;

public class BlogReadsProfile : Profile
{
    public BlogReadsProfile()
    {
        CreateMap<Blog, GetAllBlogsResponse>();
        CreateMap<Blog, GetBlogByIdResponse>();
        CreateMap<Author, GetBlogContributorsResponse>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
            .ForMember(dest => dest.ContributorId, opt => opt.MapFrom(src => src.Id));
    }
}