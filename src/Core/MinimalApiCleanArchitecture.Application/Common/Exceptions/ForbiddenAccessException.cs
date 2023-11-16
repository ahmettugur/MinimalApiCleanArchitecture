using System.Runtime.Serialization;

namespace MinimalApiCleanArchitecture.Application.Common.Exceptions;
public class ForbiddenAccessException:Exception
{
    public ForbiddenAccessException(string message) : base(message) { }
    
}