using System.Text.Json.Serialization;

namespace MinimalApiCleanArchitecture.Application.Common.Results;

public class DataResult<T> : Result, IDataResult<T>
{

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T Data { get; }

    protected DataResult(T data, bool success, string message) : base(success, message)
    {
        Data = data;
    }

    protected DataResult(T data, bool success) : base(success)
    {
        Data = data;
    }
}