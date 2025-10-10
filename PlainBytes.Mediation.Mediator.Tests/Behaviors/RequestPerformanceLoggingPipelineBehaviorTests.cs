using Microsoft.Extensions.Logging;
using PlainBytes.Mediation.Mediator.Behaviors;
using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Tests.Behaviors
{
    public class RequestPerformanceLoggingPipelineBehaviorTests
    {
        public class TestRequest : IRequest<string> { }

        [Fact]
        public async Task Handle_When_NextSucceeds_Then_LogsInformationWithTime()
        {
            // Arrange
            var logger = A.Fake<ILogger<RequestLoggingPipelineBehavior<TestRequest, string>>>();
            var behavior = new RequestPerformanceLoggingPipelineBehavior<TestRequest, string>(logger);
            var request = new TestRequest();
            var expectedResult = "success";

            // Act
            var result = await behavior.Handle(request, async () =>
            {
                await Task.Delay(10);
                return expectedResult;
            }, CancellationToken.None);

            // Assert
            Assert.Equal(expectedResult, result);
            A.CallTo(logger).Where(call =>
                call.Method.Name == "Log" &&
                call.GetArgument<LogLevel>(0) == LogLevel.Information &&
                call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2).Any(kvp =>
                    kvp.Key == "requestName" && kvp.Value.ToString()!.Contains("TestRequest")))
                .MustHaveHappened();
        }

        [Fact]
        public async Task Handle_When_NextSucceeds_Then_ReturnsResult()
        {
            // Arrange
            var logger = A.Fake<ILogger<RequestLoggingPipelineBehavior<TestRequest, string>>>();
            var behavior = new RequestPerformanceLoggingPipelineBehavior<TestRequest, string>(logger);
            var request = new TestRequest();
            var expectedResult = "test result";

            // Act
            var result = await behavior.Handle(request, () => ValueTask.FromResult(expectedResult), CancellationToken.None);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task Handle_When_NextThrows_Then_LogsErrorWithTime()
        {
            // Arrange
            var logger = A.Fake<ILogger<RequestLoggingPipelineBehavior<TestRequest, string>>>();
            var behavior = new RequestPerformanceLoggingPipelineBehavior<TestRequest, string>(logger);
            var request = new TestRequest();
            var expectedException = new InvalidOperationException("Test failure");

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await behavior.Handle(request, async () =>
                {
                    await Task.Delay(10);
                    throw expectedException;
                }, CancellationToken.None));

            Assert.Same(expectedException, actualException);
            A.CallTo(logger).Where(call =>
                call.Method.Name == "Log" &&
                call.GetArgument<LogLevel>(0) == LogLevel.Error &&
                call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2).Any(kvp =>
                    kvp.Key == "requestName" && kvp.Value.ToString()!.Contains("TestRequest")))
                .MustHaveHappened();
        }

        [Fact]
        public async Task Handle_When_NextThrows_Then_LogsTimeInErrorMessage()
        {
            // Arrange
            var logger = A.Fake<ILogger<RequestLoggingPipelineBehavior<TestRequest, string>>>();
            var behavior = new RequestPerformanceLoggingPipelineBehavior<TestRequest, string>(logger);
            var request = new TestRequest();
            var expectedException = new Exception("Test");

            // Act
            await Assert.ThrowsAsync<Exception>(
                async () => await behavior.Handle(request, async () =>
                {
                    await Task.Delay(10);
                    throw expectedException;
                }, CancellationToken.None));

            // Assert - verify time is logged in error
            A.CallTo(logger).Where(call =>
                call.Method.Name == "Log" &&
                call.GetArgument<LogLevel>(0) == LogLevel.Error &&
                call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2).Any(kvp =>
                    kvp.Key == "time"))
                .MustHaveHappened();
        }

        [Fact]
        public async Task Handle_When_NextSucceeds_Then_LogsTimeInInformation()
        {
            // Arrange
            var logger = A.Fake<ILogger<RequestLoggingPipelineBehavior<TestRequest, string>>>();
            var behavior = new RequestPerformanceLoggingPipelineBehavior<TestRequest, string>(logger);
            var request = new TestRequest();

            // Act
            await behavior.Handle(request, async () =>
            {
                await Task.Delay(10);
                return "result";
            }, CancellationToken.None);

            // Assert - verify time is logged
            A.CallTo(logger).Where(call =>
                call.Method.Name == "Log" &&
                call.GetArgument<LogLevel>(0) == LogLevel.Information &&
                call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2).Any(kvp =>
                    kvp.Key == "time"))
                .MustHaveHappened();
        }

        [Fact]
        public async Task Handle_When_CancellationTokenPassed_Then_PassedToNext()
        {
            // Arrange
            var logger = A.Fake<ILogger<RequestLoggingPipelineBehavior<TestRequest, string>>>();
            var behavior = new RequestPerformanceLoggingPipelineBehavior<TestRequest, string>(logger);
            var request = new TestRequest();
            var cts = new CancellationTokenSource();
            var nextCalled = false;

            // Act
            await behavior.Handle(request, () =>
            {
                nextCalled = true;
                return ValueTask.FromResult("result");
            }, cts.Token);

            // Assert
            Assert.True(nextCalled);
        }
    }
}
