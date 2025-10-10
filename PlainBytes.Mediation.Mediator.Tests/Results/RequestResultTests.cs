using PlainBytes.Mediation.Mediator.Results;

namespace PlainBytes.Mediation.Mediator.Tests.Results;

public class RequestResultTests
{
    [Fact]
    public void Successful_ShouldSetSuccessAndValue()
    {
        // Arrange
        var expected = 42;

        // Act
        var result = RequestResult<int>.Successful(expected);

        // Assert
        Assert.True(result.Success);
        Assert.False(result.Error);
        Assert.Null(result.Exception);
        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public void Failure_ShouldSetErrorAndException()
    {
        // Arrange
        var ex = new InvalidOperationException("fail");

        // Act
        var result = RequestResult<int>.Failure(ex);

        // Assert
        Assert.False(result.Success);
        Assert.True(result.Error);
        Assert.Equal(ex, result.Exception);
        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }

    [Fact]
    public void Failure_ShouldThrowIfExceptionIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => RequestResult<int>.Failure(null!));
    }

    [Fact]
    public void ImplicitOperator_ReturnsValueIfSuccess()
    {
        // Arrange
        var expected = "test";
        RequestResult<string> result = RequestResult<string>.Successful(expected);

        // Act
        string value = result;

        // Assert
        Assert.Equal(expected, value);
    }

    [Fact]
    public void ImplicitOperator_ThrowsIfFailure()
    {
        // Arrange
        var ex = new Exception("fail");
        var result = RequestResult<string>.Failure(ex);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => { string _ = result; });
    }

    [Fact]
    public void Deconstruct_SuccessfulResult()
    {
        // Arrange
        var expected = 123;
        var result = RequestResult<int>.Successful(expected);

        // Act
        result.Deconstruct(out var success, out var value, out var exception);

        // Assert
        Assert.True(success);
        Assert.Equal(expected, value);
        Assert.Null(exception);
    }

    [Fact]
    public void Deconstruct_FailedResult()
    {
        // Arrange
        var ex = new Exception("fail");
        var result = RequestResult<int>.Failure(ex);

        // Act
        result.Deconstruct(out var success, out var value, out var exception);

        // Assert
        Assert.False(success);
        Assert.Equal(0, value);
        Assert.Equal(ex, exception);
    }

    [Fact]
    public void TryGetValue_ReturnsTrueAndValueIfSuccess()
    {
        // Arrange
        var expected = 7;
        var result = RequestResult<int>.Successful(expected);

        // Act
        var gotValue = result.TryGetValue(out var value);

        // Assert
        Assert.True(gotValue);
        Assert.Equal(expected, value);
    }

    [Fact]
    public void TryGetValue_ReturnsFalseAndDefaultIfFailure()
    {
        // Arrange
        var ex = new Exception();
        var result = RequestResult<int>.Failure(ex);

        // Act
        var gotValue = result.TryGetValue(out var value);

        // Assert
        Assert.False(gotValue);
        Assert.Equal(0, value);
    }

    [Fact]
    public void ToString_ReturnsValueStringIfSuccess()
    {
        // Arrange
        var result = RequestResult<string>.Successful("abc");

        // Act
        var str = result.ToString();

        // Assert
        Assert.Contains("Result: abc", str);
    }

    [Fact]
    public void ToString_ReturnsExceptionStringIfFailure()
    {
        // Arrange
        var ex = new Exception("fail");
        var result = RequestResult<string>.Failure(ex);

        // Act
        var str = result.ToString();

        // Assert
        Assert.Contains("Exception: ", str);
        Assert.Contains("fail", str);
    }
}