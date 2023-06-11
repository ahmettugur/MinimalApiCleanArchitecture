using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using MinimalApiCleanArchitecture.Application.Common.Exceptions;
using Newtonsoft.Json;
using Xunit;

namespace MinimalApiCleanArchitecture.Application.UnitTests.Common.Exceptions;

public class NotFoundExceptionTests
{
    [Fact]
    public void NotfoundException_CreatesAnException()
    {
        var actual = new NotFoundException();
        actual.ErrorMessage.Should().BeEmpty();

        actual = new NotFoundException("Book not found");
        actual.ErrorMessage.Should().NotBeEmpty();
        
        actual = new NotFoundException("Book not found",new Exception("Book cannot found with book id: 1"));
        actual.ErrorMessage.Should().NotBeEmpty();
        actual.InnerException!.Message.Should().Be("Book cannot found with book id: 1");

        actual = new NotFoundException("book", "id");
        actual.ErrorMessage.Should().BeEmpty();
        actual.Message.Should().Be("Entity \"book\" (id) was not found.");
        


    }
    
    [Fact]
    public void TestNotFoundException_NotFoundExceptionWhenSerialized_ThenDeserializeCorrectly()
    {
        var actual = new NotFoundException("Not found");
        var info = new SerializationInfo(typeof(NotFoundException),new FormatterConverter());
        var context = new StreamingContext();
        actual.GetObjectData(info, context);
        actual.Should().NotBeNull();
    }
}