using PlainBytes.Mediation.Mediator.Contracts;
using PlainBytes.Mediation.Mediator.Results;

namespace PlainBytes.Mediation.Mediator.Tests.Results;

public class RequestResultExtensionsTests
{
    [Fact]
    public void FromException_ReturnsFailureResult()
    {
        // Arrange
        var ex = new InvalidOperationException("error");

        // Act
        var result = ex.FromException<int>();

        // Assert
        Assert.False(result.Success);
        Assert.True(result.Error);
        Assert.Equal(ex, result.Exception);
    }

    [Fact]
    public void ToResult_ReturnsSuccessResult()
    {
        // Arrange
        var value = 123;

        // Act
        var result = value.ToResult();

        // Assert
        Assert.True(result.Success);
        Assert.False(result.Error);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public async Task TrySend_Generic_Successful()
    {
        // Arrange
        var mediator = A.Fake<IMediator>();
        var command = A.Fake<IRequest<string>>();
        A.CallTo(() => mediator.Send(command, A<CancellationToken>._))
            .Returns("ok");

        // Act
        var result = await mediator.TrySend(command);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("ok", result.Value);
    }

    [Fact]
    public async Task TrySend_Generic_Exception()
    {
        // Arrange
        var mediator = A.Fake<IMediator>();
        var command = A.Fake<IRequest<int>>();
        var ex = new Exception("fail");
        A.CallTo(() => mediator.Send(command, A<CancellationToken>._))
            .ThrowsAsync(ex);

        // Act
        var result = await mediator.TrySend(command);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ex, result.Exception);
    }

    [Fact]
    public async Task TrySend_NonGeneric_Successful()
    {
        // Arrange
        var mediator = A.Fake<IMediator>();
        var command = A.Fake<IRequest>();
        A.CallTo(() => mediator.Send(command, A<CancellationToken>._))
            .Returns(ValueTask.CompletedTask);

        // Act
        var result = await mediator.TrySend(command);

        // Assert
        Assert.True(result.Success);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task TrySend_NonGeneric_Exception()
    {
        // Arrange
        var mediator = A.Fake<IMediator>();
        var command = A.Fake<IRequest>();
        var ex = new Exception("fail");
        A.CallTo(() => mediator.Send(command, A<CancellationToken>._))
            .ThrowsAsync(ex);

        // Act
        var result = await mediator.TrySend(command);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ex, result.Exception);
    }

    [Fact]
    public async Task TryGet_Successful()
    {
        // Arrange
        var mediator = A.Fake<IMediator>();
        var query = A.Fake<IQuery<int>>();
        A.CallTo(() => mediator.Get(query, A<CancellationToken>._))
            .Returns(42);

        // Act
        var result = await mediator.TryGet(query);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public async Task TryGet_Exception()
    {
        // Arrange
        var mediator = A.Fake<IMediator>();
        var query = A.Fake<IQuery<string>>();
        var ex = new Exception("fail");
        A.CallTo(() => mediator.Get(query, A<CancellationToken>._))
            .ThrowsAsync(ex);

        // Act
        var result = await mediator.TryGet(query);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ex, result.Exception);
    }
}