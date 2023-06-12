using FluentAssertions;
using MinimalApiCleanArchitecture.Application.Common.Results;
using MinimalApiCleanArchitecture.Domain.Model;
using Xunit;

namespace MinimalApiCleanArchitecture.Application.UnitTests.Common.Results;

public class ResultTest
{
    private Author? _author;

    public ResultTest()
    {
        _author = new Author
        {
            Id = Guid.NewGuid(), FirstName = "Jon", LastName = "Doe", Bio = "Developer",
            DateOfBirth = new DateTime(1990, 9, 1)
        };
    }

    [Fact]
    public void SetDataResult_DataResultShould_SetData()
    {
        var successDataResult = new SuccessDataResult<Author?>();
        successDataResult.Data.Should().BeNull();

        successDataResult = new SuccessDataResult<Author?>(_author);
        successDataResult.Data.Should().NotBeNull();


        successDataResult = new SuccessDataResult<Author?>("Success");
        successDataResult.Message.Should().NotBeEmpty();

        successDataResult = new SuccessDataResult<Author?>(_author, "Success");
        successDataResult.Data.Should().NotBeNull();
        successDataResult.Message.Should().NotBeEmpty();


        _author = null;

        var errorDataResult = new ErrorDataResult<Author?>();
        errorDataResult.Data.Should().BeNull();

        errorDataResult = new ErrorDataResult<Author?>(_author);
        errorDataResult.Data.Should().BeNull();

        errorDataResult = new ErrorDataResult<Author?>("Error");
        errorDataResult.Message.Should().NotBeEmpty();

        errorDataResult = new ErrorDataResult<Author?>(_author, "Error");
        errorDataResult.Data.Should().BeNull();
        errorDataResult.Message.Should().NotBeEmpty();

        var errorResult = new ErrorResult(false);
        errorResult.Success.Should().BeFalse();
        errorResult.Message.Should().BeEmpty();

        errorResult = new ErrorResult("Error");
        errorResult.Success.Should().BeFalse();
        errorResult.Message.Should().NotBeEmpty();
        
        var successResult = new SuccessResult(true);
        successResult.Success.Should().BeTrue();
        successResult.Message.Should().BeEmpty();

        successResult = new SuccessResult("Success");
        successResult.Success.Should().BeTrue();
        successResult.Message.Should().NotBeEmpty();
    }
}