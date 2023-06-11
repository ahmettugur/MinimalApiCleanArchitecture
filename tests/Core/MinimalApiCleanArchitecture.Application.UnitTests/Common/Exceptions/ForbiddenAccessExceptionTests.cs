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
    public void TestForbiddenAccessException_ForbiddenAccessExceptionWhenSerialized_ThenDeserializeCorrectly()
    {
        var exception = new ForbiddenAccessException("Forbidden access");
        var result = JsonSerializer.Serialize(exception);
        result.Should().NotBeEmpty();
        exception = JsonSerializer.Deserialize<ForbiddenAccessException>(result)!;
        exception.Message.Should().NotBeEmpty();
        
        exception = new ForbiddenAccessException("Not found");
        SerializationInfo info = null;
        var context = new StreamingContext();
        Assert.ThrowsAny<ArgumentNullException>(() =>exception.GetObjectData(info, context));
    }
}