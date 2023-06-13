using AutoMapper;
using FluentAssertions;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.GrpcService.Protos;
using MinimalApiCleanArchitecture.Infrastructure.Services.GrpcServices.AuthorGrpc;
using Moq;

namespace MinimalApiCleanArchitecture.Infrastructure.UnitTests.Services.GrpcServices.AuthorGrpc;

public class AuthorGrpcServiceTests
{
    private readonly Mock<AuthorProtoService.AuthorProtoServiceClient> _authorProtoServiceMock;
    private readonly IMapper _mapper;
    private AuthorGrpcService? _authorGrpcService;
    private readonly GetAllAuthorsProtoResponse _authorsProtoResponse;
    
    public AuthorGrpcServiceTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        
        _authorProtoServiceMock = new Mock<AuthorProtoService.AuthorProtoServiceClient>();
        IServiceCollection services = new ServiceCollection();
        services.InfrastructureServices(configuration);
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        _mapper = serviceProvider.GetService<IMapper>()!;

        var authors = new RepeatedField<AuthorProtoModel>()
        {
            new AuthorProtoModel()
            {
                Id = Guid.NewGuid().ToString(), Name = "Jon Doe", Bio = "Developer",
                DateOfBirth = Timestamp.FromDateTime(new DateTime(1990, 9, 1).ToUniversalTime())
            }
        };
        _authorsProtoResponse = new GetAllAuthorsProtoResponse()
        {
            Authors = {authors}
        };
    }

    // [Fact]
    // public async Task TestGetAllAuthors_GetAllAuthorsShouldReturn_GetAllAuthors()
    // {
    //     _authorProtoServiceMock
    //         .Setup(x => x.GetAuthorsAsync(new Empty(), null, null, default))
    //         .Returns(() => _authorsProtoResponse);
    //     _authorGrpcService = new AuthorGrpcService(_authorProtoServiceMock.Object,_mapper);
    //
    //     var result = await _authorGrpcService.GetAuthorsAsync();
    //
    //     result.Count.Should().BeGreaterThan(0);
    // }
}