using Elastic.Transport;


namespace MinimalApiCleanArchitecture.LogConsumer.Models;

public class IndexResponseModel
{
    private bool _isValid = false;
    public bool IsValid { get { return ApiCall?.HttpStatusCode == 200 ? true : _isValid; } set { _isValid = value; } }
    public string? StatusMessage { get { return ApiCall?.ToString(); } }
    public Exception? Exception { get { return ApiCall?.OriginalException; } }
    public ApiCallDetails? ApiCall { get; set; }

    public bool TryGetServerErrorReason(out string reason)
    {
        reason = "";
        return false;
    }
}