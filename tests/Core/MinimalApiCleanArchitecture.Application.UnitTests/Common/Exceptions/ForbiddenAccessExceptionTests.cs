using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using Xunit;


namespace MinimalApiCleanArchitecture.Application.UnitTests.Common.Exceptions;

public class ForbiddenAccessExceptionTests
{
    [Fact]
    public void ForbiddenAccessException_CreatesAnException()
    {
        var actual = new ForbiddenAccessException("Forbidden access");
        actual.Message.Should().Be("Forbidden access");
    }
    
    [Fact]
    public void TestForbiddenAccessException_ForbiddenAccessExceptionShould_GetObjectDataCorrectly()
    {
        var exception = new ForbiddenAccessException("Forbidden access");
        var info = new SerializationInfo(typeof(NotFoundException),new FormatterConverter());
        var context = new StreamingContext();
        exception.GetObjectData(info, context);

        exception.Should().NotBeNull();
    }
    [Fact]
    public void TestForbiddenAccessException_ForbiddenAccessExceptionWhenSerialized_ThenDeserializeCorrectly()
    {
        var exception = new ForbiddenAccessException("Forbidden access");
        var result = JsonSerializer.Serialize(exception);
        result.Should().NotBeEmpty();
        exception = JsonSerializer.Deserialize<ForbiddenAccessException>(result)!;
        exception.Message.Should().NotBeEmpty();

        exception.Should().NotBeNull();
    }
}