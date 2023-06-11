namespace MinimalApiCleanArchitecture.Application.Common.Results;

public class SuccessResult : Result
{
    public SuccessResult(string message) : base(true, message)
    {
    }

    public SuccessResult(bool success) : base(true)
    {

    }
}