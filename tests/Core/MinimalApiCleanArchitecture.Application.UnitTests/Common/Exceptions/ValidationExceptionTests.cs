using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using FluentValidation.Results;
using MinimalApiCleanArchitecture.Application.Common.Error.Response;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using Xunit;

namespace MinimalApiCleanArchitecture.Application.UnitTests.Common.Exceptions;

public class ValidationExceptionTests
{
    [Fact]
    public void DefaultConstructor_EmptyError_CreatesAnEmptyException()
    {
        var actual = new ValidationException().ValidationErrorResponse;
        actual?.StatusCode.Should().Be(422);
        actual?.StatusPhrase.Should().Be("Bad request");
        actual?.Timestamp.ToShortDateString().Should().Be(DateTime.Now.ToShortDateString());
        actual?.Errors.Count.Should().Be(0);
    }
    
    [Fact]
    public void DefaultConstructor_EmptyError_CreatesException()
    {
        var actual = new ValidationException();
        actual.Message.Should().Be("One or more validation failures have occurred.");
        actual.ValidationErrorResponse?.Should().NotBeNull();
        actual.ValidationErrorResponse?.StatusCode.Should().Be(422);
        actual.ValidationErrorResponse?.StatusPhrase.Should().Be("Bad request");
        actual.ValidationErrorResponse?.Timestamp.ToShortDateString().Should().Be(DateTime.Now.ToShortDateString());
        actual.ValidationErrorResponse?.Errors.Count.Should().Be(0);
    }
    

    [Fact]
    public void SingleValidationFailure_SingeElement_CreatesASingleElementError()
    {
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("name", "name cannot be empty"),
        };
        
        var actual = new ValidationException(failures).ValidationErrorResponse;

        actual?.Should().NotBeNull();
        actual?.Errors.FirstOrDefault()!.PropertyName.Should().BeEquivalentTo("name");
        actual?.Errors.FirstOrDefault()!.ErrorMessage.Should().BeEquivalentTo("name cannot be empty");
    }

    [Fact]
    public void ValidationFailure_GenerateValidationErrorResponse()
    {
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("name", "name cannot be empty"),
        };
        
        var actual = new ValidationException(failures).ValidationErrorResponse;
        actual?.StatusCode.Should().Be(422);
        actual?.Errors.Should().NotBeNull();
        actual?.Errors.Count.Should().BeGreaterThan(0);

    }
    
    [Fact]
    public void ValidationFailure_GenerateErrorResponse()
    {
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("name", "name cannot be empty"),
        };
        
        var actual = new ValidationException(failures).ValidationErrorResponse;
        var response = new ErrorResponse
        {
            StatusCode = actual!.StatusCode,
            Errors = { actual.Errors[0].ErrorMessage! },
            StatusPhrase = actual.StatusPhrase,
            Timestamp = actual.Timestamp
        };


        response.StatusCode.Should().Be(422);
        response.Errors.Should().NotBeNull();
        response.Errors.Count.Should().BeGreaterThan(0);

    }
}