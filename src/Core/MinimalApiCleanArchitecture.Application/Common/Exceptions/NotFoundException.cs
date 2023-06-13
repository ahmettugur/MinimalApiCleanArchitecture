using System.Runtime.Serialization;

namespace MinimalApiCleanArchitecture.Application.Common.Exceptions;

[Serializable] 
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

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorMessage = message;
    }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
    
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        HelpLink = HelpLink?.ToLower();
        Source = Source?.ToUpperInvariant();

        base.GetObjectData(info, context);
    }
}