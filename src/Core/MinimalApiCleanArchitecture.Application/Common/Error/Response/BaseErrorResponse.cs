namespace MinimalApiCleanArchitecture.Application.Common.Error.Response;

public class BaseErrorResponse
{
    public int StatusCode {get; set;}
    public string? StatusPhrase { get; set;}
    public DateTime Timestamp { get; set; }
}