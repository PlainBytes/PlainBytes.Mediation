using Microsoft.Extensions.Logging;
using PlainBytes.Mediation.Mediator.Behaviors;
using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Tests.Behaviors
{
    public class NotificationLoggingPipelineBehaviorTests
    {
        [Fact]
        public async Task Handle_When_NextSucceeds_Then_CompletesSuccessfully()
        {
            // Arrange
            var logger = A.Fake<ILogger<NotificationLoggingPipelineBehavior<TestNotification>>>();
            var behavior = new NotificationLoggingPipelineBehavior<TestNotification>(logger);
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
            A.CallTo(logger).Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Error)
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task Handle_When_NextThrows_Then_LogsErrorAndRethrows()
        {
            // Arrange
            var logger = A.Fake<ILogger<NotificationLoggingPipelineBehavior<TestNotification>>>();
            var behavior = new NotificationLoggingPipelineBehavior<TestNotification>(logger);
            var notification = new TestNotification();
            var expectedException = new InvalidOperationException("Test failure");

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await behavior.Handle(notification, () => ValueTask.FromException(expectedException), CancellationToken.None));

            Assert.Same(expectedException, actualException);
            A.CallTo(logger).Where(call =>
                call.Method.Name == "Log" &&
                call.GetArgument<LogLevel>(0) == LogLevel.Error)
                .MustHaveHappened();
        }

        [Fact]
        public async Task Handle_When_NextThrows_Then_LogsNotificationName()
        {
            // Arrange
            var logger = A.Fake<ILogger<NotificationLoggingPipelineBehavior<TestNotification>>>();
            var behavior = new NotificationLoggingPipelineBehavior<TestNotification>(logger);
            var notification = new TestNotification();
            var expectedException = new Exception("Test");

            // Act
            await Assert.ThrowsAsync<Exception>(
                async () => await behavior.Handle(notification, () => ValueTask.FromException(expectedException), CancellationToken.None));

            // Assert - verify the notification type name is logged
            A.CallTo(logger).Where(call =>
                call.Method.Name == "Log" &&
                call.GetArgument<LogLevel>(0) == LogLevel.Error &&
                call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.Any(kvp =>
                    kvp.Key == "request" && kvp.Value.ToString()!.Contains("TestNotification")))
                .MustHaveHappened();
        }

        [Fact]
        public async Task Handle_When_CancellationTokenPassed_Then_PassedToNext()
        {
            // Arrange
            var logger = A.Fake<ILogger<NotificationLoggingPipelineBehavior<TestNotification>>>();
            var behavior = new NotificationLoggingPipelineBehavior<TestNotification>(logger);
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
        public async Task Handle_When_NextThrowsMultipleTimes_Then_LogsEachTime()
        {
            // Arrange
            var logger = A.Fake<ILogger<NotificationLoggingPipelineBehavior<TestNotification>>>();
            var behavior = new NotificationLoggingPipelineBehavior<TestNotification>(logger);
            var notification = new TestNotification();

            // Act
            await Assert.ThrowsAsync<Exception>(
                async () => await behavior.Handle(notification, () => ValueTask.FromException(new Exception("Error 1")), CancellationToken.None));

            await Assert.ThrowsAsync<Exception>(
                async () => await behavior.Handle(notification, () => ValueTask.FromException(new Exception("Error 2")), CancellationToken.None));

            // Assert
            A.CallTo(logger).Where(call =>
                call.Method.Name == "Log" &&
                call.GetArgument<LogLevel>(0) == LogLevel.Error)
                .MustHaveHappened(2, Times.Exactly);
        }
    }
}
