using MinimalApiCleanArchitecture.Application.Common.Error.Response;

namespace MinimalApiCleanArchitecture.MinimalApi.Filters;

public class GuidValidationFilter : IEndpointFilter
{
    
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, 
        EndpointFilterDelegate next)
    {
        foreach (var keyValuePair in context.HttpContext.Request.RouteValues)
        {
            if (keyValuePair.Key.EndsWith("Id") || keyValuePair.Key == "id")
            {
                var isValid = IsValidGuid(keyValuePair.Value?.ToString());
                if (!isValid)
                {
                    var errorResponse = GenerateErrorResponse(new ValidationErrorResponse.ValidationError
                    {
                        PropertyName = keyValuePair.Key,
                        ErrorMessage = "Identifier not in correct GUID format"
                        
                    });
                    return Results.Json(errorResponse, statusCode: errorResponse.StatusCode);
                }
            }
        }
        
        return await next(context);
    }

    private static bool IsValidGuid(string? id)
    {
        return Guid.TryParse(id, out var value);
    }

    private static ValidationErrorResponse GenerateErrorResponse(ValidationErrorResponse.ValidationError validationError)
    {
        var apiError = new ValidationErrorResponse()
        {
            StatusCode = 400,
            StatusPhrase = "Bad request",
            Timestamp = DateTime.Now,
            Errors = { validationError }
        };

        return apiError;
    }
}