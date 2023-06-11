using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.CreateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.DeleteAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Commands.UpdateAuthor;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAllAuthors;
using MinimalApiCleanArchitecture.Application.Features.AuthorFeature.Queries.GetAuthorById;
using MinimalApiCleanArchitecture.GrpcService.Protos;
using Newtonsoft.Json;

namespace MinimalApiCleanArchitecture.GrpcService.Services;

public class AuthorService : AuthorProtoService.AuthorProtoServiceBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public AuthorService(IMediator mediator, IMapper mapper, ILogger<AuthorService> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public override async Task<GetAllAuthorsProtoResponse> GetAuthors(Empty request, ServerCallContext context)
    {
        var result = await _mediator.Send(new GetAllAuthorsQuery());
        var authors = _mapper.Map<List<AuthorProtoModel>>(result);
        return new GetAllAuthorsProtoResponse
        {
            Authors = {authors}
        };
    }

    public override async Task<GetAuthorByIdProtoResponse> GetAuthorById(GetAuthorByIdProtoRequest request, ServerCallContext context)
    {
        try
        {
            var result = await _mediator.Send(new GetAuthorByIdQuery(Guid.Parse(request.AuthorId)));

            var author = _mapper.Map<AuthorProtoModel>(result);
            return new GetAuthorByIdProtoResponse
            {
                Author = author
            };
        }
        catch (NotFoundException ex)
        {
            var metadata = new Metadata
            {
                {"exception-type", "NotFoundException"},
                {"original-exception", JsonConvert.SerializeObject(ex)}
            };
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message, ex), metadata);
        }
        catch (Exception ex)
        {
            var metadata = new Metadata
            {
                {"exception-type", "Exception"}
            };
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex), metadata);
        }
    }

    public override async Task<CreateAuthorProtoResponse> CreateAuthor(CreateAuthorProtoRequest request, ServerCallContext context)
    {
        try
        {
            var command = new CreateAuthorCommand(request.FirstName, request.LastName, request.Bio,
                request.DateOfBirth.ToDateTime());
            var result = await _mediator.Send(command);
            var author = _mapper.Map<AuthorProtoModel>(result);
            return new CreateAuthorProtoResponse {Author = author};
        }
        catch (ValidationException ex)
        {
            var metadata = new Metadata
            {
                {"exception-type", "ValidationException"},
                {"original-exception", JsonConvert.SerializeObject(ex)}
            };
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message, ex), metadata);
        }
        catch (Exception ex)
        {
            var metadata = new Metadata
            {
                {"exception-type", "Exception"}
            };
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex), metadata);
        }
    }

    public override async Task<UpdateAuthorProtoResponse> UpdateAuthor(UpdateAuthorProtoRequest request,ServerCallContext context)
    {
        try
        {
            var command = new UpdateAuthorCommand(Guid.Parse(request.Id), request.FirstName, request.LastName,
                request.Bio, request.DateOfBirth.ToDateTime());
            await _mediator.Send(command);
            return new UpdateAuthorProtoResponse {Status = true};
        }
        catch (NotFoundException ex)
        {
            var metadata = new Metadata
            {
                {"exception-type", "NotFoundException"},
                {"original-exception", JsonConvert.SerializeObject(ex)}
            };
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message, ex), metadata);
        }
        catch (ValidationException ex)
        {
            var metadata = new Metadata
            {
                {"exception-type", "ValidationException"},
                {"original-exception", JsonConvert.SerializeObject(ex)}
            };
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message, ex), metadata);
        }
        catch (Exception ex)
        {
            var metadata = new Metadata
            {
                {"exception-type", "Exception"}
            };
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex), metadata);
        }
    }

    public override async Task<DeleteAuthorProtoResponse> DeleteAuthor(DeleteAuthorProtoRequest request, ServerCallContext context)
    {
        try
        {
            var command = new DeleteAuthorCommand(Guid.Parse(request.Id));
            await _mediator.Send(command);
            return new DeleteAuthorProtoResponse
            {
                Status = true
            };
        }
        catch (NotFoundException ex)
        {
            var metadata = new Metadata
            {
                {"exception-type", "NotFoundException"},
                {"original-exception", JsonConvert.SerializeObject(ex)}
            };
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message, ex), metadata);
        }
        catch (Exception ex)
        {
            var metadata = new Metadata
            {
                {"exception-type", "Exception"}
            };
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex), metadata);
        }
    }
}