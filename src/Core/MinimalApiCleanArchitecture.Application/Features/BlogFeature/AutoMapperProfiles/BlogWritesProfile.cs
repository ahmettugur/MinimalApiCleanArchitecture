using AutoMapper;
using MinimalApiCleanArchitecture.Application.Features.BlogFeature.Commands.CreateBlog;
using MinimalApiCleanArchitecture.Domain.Model;

namespace MinimalApiCleanArchitecture.Application.Features.BlogFeature.AutoMapperProfiles;

public class BlogWritesProfile: Profile
{
    public BlogWritesProfile()
    {
        CreateMap<Blog, CreateBlogResponse>();
        CreateMap<CreateBlogRequest, CreateBlogCommand>().ReverseMap();
    }
}