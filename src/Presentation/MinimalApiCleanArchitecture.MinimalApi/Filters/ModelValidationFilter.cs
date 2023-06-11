using FluentValidation;
using FluentValidation.Results;
using MinimalApiCleanArchitecture.Application.Common.Error.Response;

namespace MinimalApiCleanArchitecture.MinimalApi.Filters;

public class ModelValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;

    public ModelValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var model = context.Arguments
            .FirstOrDefault(a => a?.GetType() == typeof(T)) as T;
        if (model is null)
        {
            var errorResponse = GenerateErrorResponse(new List<ValidationFailure>
            {
                new("body", "Request body not in correct format")
            });

            return Results.Json(errorResponse, statusCode: errorResponse.StatusCode);
        }

        var validationResult = await _validator.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            var errorResponse = GenerateErrorResponse(validationResult.Errors);
            return Results.Json(errorResponse, statusCode: errorResponse.StatusCode);
        }

        return await next(context);
    }

    private static ValidationErrorResponse GenerateErrorResponse(List<ValidationFailure> failures)
    {
        var apiError = new ValidationErrorResponse
        {
            StatusCode = 400,
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
}