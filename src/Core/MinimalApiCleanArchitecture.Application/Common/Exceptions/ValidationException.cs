using System.Runtime.Serialization;
using FluentValidation.Results;
using MinimalApiCleanArchitecture.Application.Common.Error.Response;

namespace MinimalApiCleanArchitecture.Application.Common.Exceptions;

[Serializable]
public class ValidationException : Exception
{
    public ValidationErrorResponse? ValidationErrorResponse { get; set; }
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        ValidationErrorResponse = new ValidationErrorResponse
        {
            StatusCode = 422,
            StatusPhrase = "Bad request",
            Timestamp = DateTime.Now
        };
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        ValidationErrorResponse = GenerateErrorResponse(failures.ToList());
    }
    private static ValidationErrorResponse GenerateErrorResponse(List<ValidationFailure> failures)
    {
        var apiError = new ValidationErrorResponse
        {
            StatusCode = 422,
            StatusPhrase = "Bad request",
            Timestamp = DateTime.Now
            
        };
        failures.ForEach(e => apiError.Errors.Add(new ValidationErrorResponse.ValidationError
        {
            PropertyName = e.PropertyName.ToLower(),
            ErrorMessage = e.ErrorMessage
        }));
        return apiError;
    }
    
    protected ValidationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ValidationErrorResponse = (ValidationErrorResponse)info.GetValue(nameof(ValidationErrorResponse), typeof(ValidationErrorResponse))!;
        
    }
    
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        HelpLink = HelpLink?.ToLower();
        Source = Source?.ToUpperInvariant();

        base.GetObjectData(info, context);
    }

}
