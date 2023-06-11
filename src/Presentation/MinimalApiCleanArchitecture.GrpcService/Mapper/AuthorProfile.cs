using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAllAuthors;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAuthorById;
using MinimalApiCleanArchitecture.GrpcService.Protos;

namespace MinimalApiCleanArchitecture.GrpcService.Mapper;

public class AuthorProfile: Profile
{
    public AuthorProfile()
    {
        CreateMap<GetAllAuthorsResponse, AuthorProtoModel>()
            .ForMember(dest => dest.DateOfBirth, opt =>
                opt.MapFrom(src => Timestamp.FromDateTime(src.DateOfBirth.ToUniversalTime())));
       
        CreateMap<GetAuthorByIdResponse, AuthorProtoModel>()
            .ForMember(dest => dest.DateOfBirth, opt =>
                opt.MapFrom(src => Timestamp.FromDateTime(src.DateOfBirth.ToUniversalTime())));

        CreateMap<CreateAuthorResponse, AuthorProtoModel>()
            .ForMember(dest => dest.DateOfBirth, opt =>
                opt.MapFrom(src => Timestamp.FromDateTime(src.DateOfBirth.ToUniversalTime())));
    }
}
