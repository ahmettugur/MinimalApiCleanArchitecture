using AutoMapper;
using Consul;
using FluentAssertions;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.DeleteAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.UpdateAuthor;
using MinimalApiCleanArchitecture.Infrastructure.Protos;
using MinimalApiCleanArchitecture.Infrastructure.Services.GrpcServices.AuthorGrpc;
using Moq;
using Status = Grpc.Core.Status;

namespace MinimalApiCleanArchitecture.Infrastructure.UnitTests.Services.GrpcServices.AuthorGrpc;

public class AuthorGrpcServiceTests
{
    private readonly Mock<AuthorProtoService.AuthorProtoServiceClient> _authorProtoServiceMock;
    private readonly IMapper _mapper;
    private AuthorGrpcService _authorGrpcService;
    private readonly AuthorProtoModel _authorProtoModel;
    private readonly GetAllAuthorsProtoResponse _authorsProtoResponse;
    private readonly GetAuthorByIdProtoResponse _authorByIdProtoResponse;
    private readonly CreateAuthorProtoResponse _createAuthorProtoResponse;
    private readonly UpdateAuthorProtoResponse _updateAuthorProtoResponse;
    private readonly DeleteAuthorProtoResponse _deleteAuthorProtoResponse;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfigurationRoot _configuration;
    private readonly Mock<ILogger<AuthorGrpcService>> _loggerMock;

    public AuthorGrpcServiceTests()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        _authorProtoServiceMock = new Mock<AuthorProtoService.AuthorProtoServiceClient>();
        _loggerMock = new Mock<ILogger<AuthorGrpcService>>();
        IServiceCollection services = new ServiceCollection();
        services.AddInfrastructureServices(_configuration);
        _serviceProvider = services.BuildServiceProvider();
        _mapper = _serviceProvider.GetService<IMapper>()!;

        _authorGrpcService = new AuthorGrpcService(_authorProtoServiceMock.Object, _mapper, _loggerMock.Object);

        _authorProtoModel = new AuthorProtoModel()
        {
            Id = Guid.NewGuid().ToString(), Name = "Jon Doe", Bio = "Developer",
            DateOfBirth = Timestamp.FromDateTime(new DateTime(1990, 9, 1,0,0,0,DateTimeKind.Utc).ToUniversalTime())
        };
        var authors = new RepeatedField<AuthorProtoModel>() {_authorProtoModel};
        _authorsProtoResponse = new GetAllAuthorsProtoResponse() {Authors = {authors}};

        _authorByIdProtoResponse = new GetAuthorByIdProtoResponse() {Author = _authorProtoModel};

        _createAuthorProtoResponse = new CreateAuthorProtoResponse() {Author = _authorProtoModel};
        _updateAuthorProtoResponse = new UpdateAuthorProtoResponse() {Status = true};
        _deleteAuthorProtoResponse = new DeleteAuthorProtoResponse() {Status = true};
    }

    [Fact]
    public void TestAddInfrastructureServices_AddInfrastructureServicesShould_GetServices()
    {
        _serviceProvider.GetService<IMapper>().Should().NotBeNull();
        _serviceProvider.GetService<IConsulClient>().Should().BeNull();
        _serviceProvider.GetService<AuthorProtoService.AuthorProtoServiceClient>().Should().NotBeNull();
        _serviceProvider.GetService<AuthorGrpcService>().Should().BeNull();

        var serviceName = _configuration["GrpcSettings:AuthorGrpcServiceConsulName"];
        serviceName.Should().NotBeNull();

        var consulClient = _serviceProvider.GetService<IConsulClient>(); 
        consulClient.Should().BeNull();
        
    }

    [Fact]
    public async Task TestGetAllAuthors_GetAllAuthorsShouldReturn_GetAllAuthors()
    {
        var mockCall = CallHelpers.CreateAsyncUnaryCall(_authorsProtoResponse);
        _authorProtoServiceMock
            .Setup(x => x.GetAuthorsAsync(new Empty(), null, null, default))
            .Returns(mockCall);

        mockCall.GetStatus().Should().Be(Status.DefaultSuccess);
        mockCall.GetTrailers().Should().NotBeNull();
        mockCall.Dispose();

        var result = await _authorGrpcService.GetAuthorsAsync();

        result.Count.Should().BeGreaterThan(0);

        _authorsProtoResponse.Authors.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task TestGetAuthorById_GetAuthorByIdShouldReturn_GetAuthor()
    {
        var mockCall = CallHelpers.CreateAsyncUnaryCall(_authorByIdProtoResponse);
        _authorProtoServiceMock
            .Setup(x => x.GetAuthorByIdAsync(new GetAuthorByIdProtoRequest {AuthorId = _authorProtoModel.Id}, null,
                null, default))
            .Returns(mockCall);
        _authorGrpcService = new AuthorGrpcService(_authorProtoServiceMock.Object, _mapper, _loggerMock.Object);

        var result = await _authorGrpcService.GetAuthorByIdAsync(_authorProtoModel.Id);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task TestCreateAuthor_CreateAuthorShouldReturn_InsertedAuthor()
    {
        var request = new CreateAuthorRequest("Jon", "Doe", "Developer", new DateTime(1990, 9, 1,0,0,0,DateTimeKind.Utc));

        var protoRequest = _mapper.Map<CreateAuthorProtoRequest>(request);

        var mockCall = CallHelpers.CreateAsyncUnaryCall(_createAuthorProtoResponse);
        _authorProtoServiceMock
            .Setup(x => x.CreateAuthorAsync(protoRequest, null, null, default))
            .Returns(mockCall);

        var result = await _authorGrpcService.CreateAuthorAsync(request);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task TestUpdateAuthor_UpdateAuthorShouldReturn_UpdateStstusTrue()
    {
        var request = new UpdateAuthorRequest(Guid.NewGuid(), "Jon", "Doe", "Developer", new DateTime(1990, 9, 1,0,0,0,DateTimeKind.Utc));
        var protoRequest = _mapper.Map<UpdateAuthorProtoRequest>(request);

        var mockCall = CallHelpers.CreateAsyncUnaryCall(_updateAuthorProtoResponse);
        _authorProtoServiceMock
            .Setup(_ => _.UpdateAuthorAsync(protoRequest, null, null, default))
            .Returns(mockCall);

        var result = await _authorGrpcService.UpdateAuthorAsync(request, request.AuthorId.ToString());

        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
    }

    [Fact]
    public async Task TestDeleteAuthor_DeleteAuthorShouldReturn_DeleteStatusTrue()
    {
        var request = new DeleteAuthorRequest(Guid.NewGuid());
        var protoRequest = _mapper.Map<DeleteAuthorProtoRequest>(request);

        var mockCall = CallHelpers.CreateAsyncUnaryCall(_deleteAuthorProtoResponse);
        _authorProtoServiceMock
            .Setup(_ => _.DeleteAuthorAsync(protoRequest, null, null, default))
            .Returns(mockCall);

        var result = await _authorGrpcService.DeleteAuthorAsync(request);

        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
    }
}