using AutoMapper;
using FluentAssertions;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.GrpcService.Protos;
using MinimalApiCleanArchitecture.Infrastructure.Services.GrpcServices.AuthorGrpc;
using Moq;

namespace MinimalApiCleanArchitecture.Infrastructure.UnitTests.Services.GrpcServices.AuthorGrpc;

public class AuthorGrpcServiceTests
{
    private readonly Mock<AuthorProtoService.AuthorProtoServiceClient> _authorProtoServiceMock;
    private readonly IMapper _mapper;
    private AuthorGrpcService? _authorGrpcService;
    private readonly AuthorProtoModel _authorProtoModel;
    private readonly GetAllAuthorsProtoResponse _authorsProtoResponse;
    private readonly GetAuthorByIdProtoResponse _authorByIdProtoResponse;
    private readonly CreateAuthorProtoResponse _createAuthorProtoResponse;
    private readonly CreateAuthorResponse _createAuthorResponse;

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

        _authorProtoModel = new AuthorProtoModel()
        {
            Id = Guid.NewGuid().ToString(), Name = "Jon Doe", Bio = "Developer",
            DateOfBirth = Timestamp.FromDateTime(new DateTime(1990, 9, 1).ToUniversalTime())
        };
        var authors = new RepeatedField<AuthorProtoModel>() {_authorProtoModel};
        _authorsProtoResponse = new GetAllAuthorsProtoResponse() {Authors = {authors}};

        _authorByIdProtoResponse = new GetAuthorByIdProtoResponse() {Author = _authorProtoModel};

        _createAuthorProtoResponse = new CreateAuthorProtoResponse() {Author = _authorProtoModel};
        _createAuthorResponse = new CreateAuthorResponse()
        {
            Id = Guid.Parse(_authorProtoModel.Id),
            Name = _authorProtoModel.Name,
            Bio = _authorProtoModel.Bio,
            DateOfBirth = _authorProtoModel.DateOfBirth.ToDateTime()
        };
    }

    [Fact]
    public async Task TestGetAllAuthors_GetAllAuthorsShouldReturn_GetAllAuthors()
    {
        var mockCall = CallHelpers.CreateAsyncUnaryCall(_authorsProtoResponse);
        _authorProtoServiceMock
            .Setup(x => x.GetAuthorsAsync(new Empty(), null, null, default))
            .Returns(mockCall);
        _authorGrpcService = new AuthorGrpcService(_authorProtoServiceMock.Object, _mapper);

        var result = await _authorGrpcService.GetAuthorsAsync();

        result.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task TestGetAuthorById_GetAuthorByIdShouldReturn_GetAuthor()
    {
        var mockCall = CallHelpers.CreateAsyncUnaryCall(_authorByIdProtoResponse);
        _authorProtoServiceMock
            .Setup(x => x.GetAuthorByIdAsync(new GetAuthorByIdProtoRequest {AuthorId = _authorProtoModel.Id}, null,
                null, default))
            .Returns(mockCall);
        _authorGrpcService = new AuthorGrpcService(_authorProtoServiceMock.Object, _mapper);

        var result = await _authorGrpcService.GetAuthorByIdAsync(_authorProtoModel.Id);

        result.Should().NotBeNull();
    }

    [Fact]
     public async Task TestCreateAuthor_CreateAuthorShouldReturn_GetAuthor()
     {
         var request = new CreateAuthorRequest("Jon","Doe","Developer",new DateTime(1990, 9, 1));
         
         var protoRequest = new CreateAuthorProtoRequest()
         {
             LastName = request.LastName, FirstName = request.FirstName, Bio = request.Bio,
             DateOfBirth = Timestamp.FromDateTime(request.DateOfBirth.ToUniversalTime())
         };
    
         var mockCall = CallHelpers.CreateAsyncUnaryCall(_createAuthorProtoResponse);
         _authorProtoServiceMock
             .Setup(x => x.CreateAuthorAsync(protoRequest, null, null, default))
             .Returns(mockCall);
         _authorGrpcService = new AuthorGrpcService(_authorProtoServiceMock.Object, _mapper);
    
         var result = await _authorGrpcService.CreateAuthorAsync(request);
    
         result.Should().NotBeNull();
     }
}