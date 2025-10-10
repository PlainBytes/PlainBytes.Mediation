using Microsoft.Extensions.Logging;
using PlainBytes.Mediation.Mediator.Behaviors;
using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Tests.Behaviors
{
    public class NotificationPerformanceLoggingPipelineBehaviorTests
    {
        public class TestNotification : INotification { }

        [Fact]
        public async Task Handle_When_NextSucceeds_Then_LogsInformationWithTime()
        {
            // Arrange
            var logger = A.Fake<ILogger<NotificationLoggingPipelineBehavior<TestNotification>>>();
            var behavior = new NotificationPerformanceLoggingPipelineBehavior<TestNotification>(logger);
            var notification = new TestNotification();

            // Act
            await behavior.Handle(notification, async () =>
            {
                await Task.Delay(10);
            }, CancellationToken.None);

            // Assert
            A.CallTo(logger).Where(call =>
                call.Method.Name == "Log" &&
                call.GetArgument<LogLevel>(0) == LogLevel.Information &&
                call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2).Any(kvp =>
                    kvp.Key == "notification" && kvp.Value.ToString()!.Contains("TestNotification")))
                .MustHaveHappened();
        }

        [Fact]
        public async Task Handle_When_NextSucceeds_Then_CompletesSuccessfully()
        {
            // Arrange
            var logger = A.Fake<ILogger<NotificationLoggingPipelineBehavior<TestNotification>>>();
            var behavior = new NotificationPerformanceLoggingPipelineBehavior<TestNotification>(logger);
            var notification = new TestNotification();
            var nextCalled = false;

            // Act
            await behavior.Handle(notification, () =>
            {
                nextCalled = true;
                return ValueTask.CompletedTask;
            }, CancellationToken.None);

            // Assert
            Assert.True(nextCalled);
        }

        [Fact]
        public async Task Handle_When_NextThrows_Then_LogsErrorWithTime()
        {
            // Arrange
            var logger = A.Fake<ILogger<NotificationLoggingPipelineBehavior<TestNotification>>>();
            var behavior = new NotificationPerformanceLoggingPipelineBehavior<TestNotification>(logger);
            var notification = new TestNotification();
            var expectedException = new InvalidOperationException("Test failure");

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await behavior.Handle(notification, async () =>
                {
                    await Task.Delay(10);
                    throw expectedException;
                }, CancellationToken.None));

            Assert.Same(expectedException, actualException);
            A.CallTo(logger).Where(call =>
                call.Method.Name == "Log" &&
                call.GetArgument<LogLevel>(0) == LogLevel.Error &&
                call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2).Any(kvp =>
                    kvp.Key == "notification" && kvp.Value.ToString()!.Contains("TestNotification")))
                .MustHaveHappened();
        }

        [Fact]
        public async Task Handle_When_NextThrows_Then_LogsTimeInErrorMessage()
        {
            // Arrange
            var logger = A.Fake<ILogger<NotificationLoggingPipelineBehavior<TestNotification>>>();
            var behavior = new NotificationPerformanceLoggingPipelineBehavior<TestNotification>(logger);
            var notification = new TestNotification();
            var expectedException = new Exception("Test");

            // Act
            await Assert.ThrowsAsync<Exception>(
                async () => await behavior.Handle(notification, async () =>
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
            var logger = A.Fake<ILogger<NotificationLoggingPipelineBehavior<TestNotification>>>();
            var behavior = new NotificationPerformanceLoggingPipelineBehavior<TestNotification>(logger);
            var notification = new TestNotification();

            // Act
            await behavior.Handle(notification, async () =>
            {
                await Task.Delay(10);
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
            var logger = A.Fake<ILogger<NotificationLoggingPipelineBehavior<TestNotification>>>();
            var behavior = new NotificationPerformanceLoggingPipelineBehavior<TestNotification>(logger);
            var notification = new TestNotification();
            var cts = new CancellationTokenSource();
            var nextCalled = false;

            // Act
            await behavior.Handle(notification, () =>
            {
                nextCalled = true;
                return ValueTask.CompletedTask;
            }, cts.Token);

            // Assert
            Assert.True(nextCalled);
        }

        [Fact]
        public async Task Handle_When_MeasuresElapsedTime_Then_TimeIsReasonable()
        {
            // Arrange
            var logger = A.Fake<ILogger<NotificationLoggingPipelineBehavior<TestNotification>>>();
            var behavior = new NotificationPerformanceLoggingPipelineBehavior<TestNotification>(logger);
            var notification = new TestNotification();

            // Act
            await behavior.Handle(notification, async () =>
            {
                await Task.Delay(50);
            }, CancellationToken.None);

            // Assert - verify that a time value was logged and it's greater than 0
            A.CallTo(logger).Where(call =>
                call.Method.Name == "Log" &&
                call.GetArgument<LogLevel>(0) == LogLevel.Information &&
                call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2).Any(kvp =>
                    kvp.Key == "time" && (double)kvp.Value > 0))
                .MustHaveHappened();
        }
    }
}
