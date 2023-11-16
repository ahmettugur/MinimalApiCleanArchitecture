using System.Runtime.Serialization;

namespace MinimalApiCleanArchitecture.Application.Common.Exceptions;
public class NotFoundException : Exception
{
    public string ErrorMessage { get; set; } = "";
    public NotFoundException()
        : base()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
        ErrorMessage = message;
    }
    
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorMessage = message;
    }

}