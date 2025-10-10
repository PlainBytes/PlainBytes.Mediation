using PlainBytes.Mediation.Mediator.Contracts;
using PlainBytes.Mediation.Mediator.Strategies;

namespace PlainBytes.Mediation.Mediator.Tests.Strategies
{
    public class ParallelStrategyTests
    {
        [Fact]
        public async Task Publish_When_MultipleHandlers_Then_ExecutesInParallel()
        {
            // Arrange
            var strategy = new ParallelStrategy();
            var notification = new TestNotification();

            var handlers = HandlersFactory.CreateNotifications().ToDictionary();

            // Act
            var publishTask = strategy.Publish(notification, handlers.Keys, CancellationToken.None);

            // Wait a bit to ensure all handlers are called
            await Task.Delay(10);

            // Complete all handlers
            foreach (var handlersValue in handlers.Values)
            {
                handlersValue.SetResult();
            }

            await publishTask;

            // Assert
            foreach (var handler in handlers.Keys)
            {
                A.CallTo(() => handler.Handle(notification, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            }
        }

        [Fact]
        public async Task Publish_When_NoHandlers_Then_CompletesSuccessfully()
        {
            // Arrange
            var strategy = new ParallelStrategy();
            var notification = new TestNotification();
            var handlers = Array.Empty<INotificationHandler<TestNotification>>();

            // Act
            await strategy.Publish(notification, handlers, CancellationToken.None);

            // Assert - should complete without exception
            Assert.True(true);
        }

        [Fact]
        public async Task Publish_When_HandlerThrows_Then_ExceptionIsPropagated()
        {
            // Arrange
            var strategy = new ParallelStrategy();
            var notification = new TestNotification();
            var handler = A.Fake<INotificationHandler<TestNotification>>();
            var expectedException = new InvalidOperationException("Handler failed");

            A.CallTo(() => handler.Handle(notification, A<CancellationToken>._))
                .Returns(ValueTask.FromException(expectedException));

            var handlers = new[] { handler };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await strategy.Publish(notification, handlers, CancellationToken.None));
        }

        [Fact]
        public async Task Publish_When_CancellationRequested_Then_PassesToHandlers()
        {
            // Arrange
            var strategy = new ParallelStrategy();
            var notification = new TestNotification();
            var handler = A.Fake<INotificationHandler<TestNotification>>();
            var cts = new CancellationTokenSource();
            
            await cts.CancelAsync();

            A.CallTo(() => handler.Handle(notification, A<CancellationToken>._))
                .Returns(ValueTask.CompletedTask);

            var handlers = new[] { handler };

            // Act
            await strategy.Publish(notification, handlers, cts.Token);

            // Assert
            A.CallTo(() => handler.Handle(notification, cts.Token)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Publish_When_HandlersCompleteImmediately_Then_ReturnsCompletedTask()
        {
            // Arrange
            var strategy = new ParallelStrategy();
            var notification = new TestNotification();
            var handler1 = A.Fake<INotificationHandler<TestNotification>>();
            var handler2 = A.Fake<INotificationHandler<TestNotification>>();

            A.CallTo(() => handler1.Handle(notification, A<CancellationToken>._))
                .Returns(ValueTask.CompletedTask);
            A.CallTo(() => handler2.Handle(notification, A<CancellationToken>._))
                .Returns(ValueTask.CompletedTask);

            var handlers = new[] { handler1, handler2 };

            // Act
            var task = strategy.Publish(notification, handlers, CancellationToken.None);

            // Assert
            Assert.True(task.IsCompleted);
            await task;
        }

        
    }
}
