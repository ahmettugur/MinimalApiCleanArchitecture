using AutoMapper;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.DeleteAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.UpdateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAllAuthors;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAuthorById;
using MinimalApiCleanArchitecture.Domain.Model;

namespace MinimalApiCleanArchitecture.Application.Features.AuthorFeature.AutoMapperProfiles;

public class AuthorProfile : Profile
{
    public AuthorProfile()
    {
        CreateMap<CreateAuthorCommand, Author>();
        CreateMap<CreateAuthorRequest, CreateAuthorCommand>().ReverseMap();
        CreateMap<UpdateAuthorRequest, UpdateAuthorCommand>().ReverseMap();
        CreateMap<DeleteAuthorRequest, DeleteAuthorCommand>().ReverseMap();
        CreateMap<Author, CreateAuthorResponse>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName)).ReverseMap();
        CreateMap<Author, GetAllAuthorsResponse>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName)).ReverseMap();
        CreateMap<Author, GetAuthorByIdResponse>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName)).ReverseMap();
    }
}