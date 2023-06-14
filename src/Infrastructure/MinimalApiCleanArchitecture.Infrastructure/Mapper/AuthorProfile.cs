using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.DeleteAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.UpdateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAllAuthors;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAuthorById;
using MinimalApiCleanArchitecture.Infrastructure.Protos;

namespace MinimalApiCleanArchitecture.Infrastructure.Mapper;

public class AuthorProfile: Profile
{
    public AuthorProfile()
    {
        CreateMap<AuthorProtoModel, GetAllAuthorsResponse>()
            .ForMember(dest => dest.DateOfBirth, opt =>
                opt.MapFrom(src => src.DateOfBirth.ToDateTime()));

        CreateMap<AuthorProtoModel, GetAuthorByIdResponse>()
            .ForMember(dest => dest.DateOfBirth, opt =>
                opt.MapFrom(src => src.DateOfBirth.ToDateTime()));

        CreateMap<AuthorProtoModel, CreateAuthorResponse>()
            .ForMember(dest => dest.DateOfBirth, opt =>
                opt.MapFrom(src => src.DateOfBirth.ToDateTime()));
        
        CreateMap<AuthorProtoModel, CreateAuthorRequest>()
            .ForMember(dest => dest.DateOfBirth, opt =>
                opt.MapFrom(src => src.DateOfBirth.ToDateTime()));


        CreateMap<CreateAuthorResponse, AuthorProtoModel>()
            .ForMember(dest => dest.DateOfBirth, opt =>
                opt.MapFrom(src => Timestamp.FromDateTime(src.DateOfBirth)));

        CreateMap<UpdateAuthorProtoResponse, UpdateAuthorResponse>();
        CreateMap<DeleteAuthorProtoResponse, DeleteAuthorResponse>();

        CreateMap<CreateAuthorRequest, CreateAuthorProtoRequest>()
            .ForMember(dest => dest.DateOfBirth, opt =>
                opt.MapFrom(src => Timestamp.FromDateTime(src.DateOfBirth.ToUniversalTime()))).ReverseMap();

        CreateMap<UpdateAuthorRequest, UpdateAuthorProtoRequest>()
            .ForMember(dest => dest.DateOfBirth, opt =>
                opt.MapFrom(src => Timestamp.FromDateTime(src.DateOfBirth.ToUniversalTime())))
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => src.AuthorId.ToString())).ReverseMap();

        CreateMap<DeleteAuthorRequest, DeleteAuthorProtoRequest>();

    }
}
