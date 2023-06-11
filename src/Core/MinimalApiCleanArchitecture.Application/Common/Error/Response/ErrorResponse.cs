namespace MinimalApiCleanArchitecture.Application.Common.Error.Response;

public class ErrorResponse:BaseErrorResponse
{
    public List<string> Errors { get; } = new List<string>();
}