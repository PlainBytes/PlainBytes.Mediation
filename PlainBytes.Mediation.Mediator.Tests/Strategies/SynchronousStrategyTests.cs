using PlainBytes.Mediation.Mediator.Contracts;
using PlainBytes.Mediation.Mediator.Strategies;

namespace PlainBytes.Mediation.Mediator.Tests.Strategies
{
    public class SynchronousStrategyTests
    {
        [Fact]
        public async Task Publish_When_MultipleHandlers_Then_ExecutesSequentially()
        {
            // Arrange
            var strategy = new SynchronousStrategy();
            var notification = new TestNotification();
            var executionOrder = new List<int>();

            var handler1 = A.Fake<INotificationHandler<TestNotification>>();
            var handler2 = A.Fake<INotificationHandler<TestNotification>>();
            var handler3 = A.Fake<INotificationHandler<TestNotification>>();

            A.CallTo(() => handler1.Handle(notification, A<CancellationToken>._))
                .ReturnsLazily(() =>
                {
                    executionOrder.Add(1);
                    return ValueTask.CompletedTask;
                });

            A.CallTo(() => handler2.Handle(notification, A<CancellationToken>._))
                .ReturnsLazily(() =>
                {
                    executionOrder.Add(2);
                    return ValueTask.CompletedTask;
                });

            A.CallTo(() => handler3.Handle(notification, A<CancellationToken>._))
                .ReturnsLazily(() =>
                {
                    executionOrder.Add(3);
                    return ValueTask.CompletedTask;
                });

            var handlers = new[] { handler1, handler2, handler3 };

            // Act
            await strategy.Publish(notification, handlers, CancellationToken.None);

            // Assert
            Assert.Equal(new[] { 1, 2, 3 }, executionOrder);
            A.CallTo(() => handler1.Handle(notification, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => handler2.Handle(notification, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => handler3.Handle(notification, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Publish_When_NoHandlers_Then_CompletesSuccessfully()
        {
            // Arrange
            var strategy = new SynchronousStrategy();
            var notification = new TestNotification();
            var handlers = Array.Empty<INotificationHandler<TestNotification>>();

            // Act
            await strategy.Publish(notification, handlers, CancellationToken.None);

            // Assert - should complete without exception
            Assert.True(true);
        }

        [Fact]
        public async Task Publish_When_SingleHandler_Then_ExecutesHandler()
        {
            // Arrange
            var strategy = new SynchronousStrategy();
            var notification = new TestNotification();
            var handler = A.Fake<INotificationHandler<TestNotification>>();

            A.CallTo(() => handler.Handle(notification, A<CancellationToken>._))
                .Returns(ValueTask.CompletedTask);

            var handlers = new[] { handler };

            // Act
            await strategy.Publish(notification, handlers, CancellationToken.None);

            // Assert
            A.CallTo(() => handler.Handle(notification, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Publish_When_HandlerThrows_Then_StopsExecutionAndPropagatesException()
        {
            // Arrange
            var strategy = new SynchronousStrategy();
            var notification = new TestNotification();
            var handler1 = A.Fake<INotificationHandler<TestNotification>>();
            var handler2 = A.Fake<INotificationHandler<TestNotification>>();
            var handler3 = A.Fake<INotificationHandler<TestNotification>>();
            var expectedException = new InvalidOperationException("Handler 2 failed");

            A.CallTo(() => handler1.Handle(notification, A<CancellationToken>._))
                .Returns(ValueTask.CompletedTask);

            A.CallTo(() => handler2.Handle(notification, A<CancellationToken>._))
                .Returns(ValueTask.FromException(expectedException));

            A.CallTo(() => handler3.Handle(notification, A<CancellationToken>._))
                .Returns(ValueTask.CompletedTask);

            var handlers = new[] { handler1, handler2, handler3 };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await strategy.Publish(notification, handlers, CancellationToken.None));

            Assert.Same(expectedException, ex);
            A.CallTo(() => handler1.Handle(notification, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => handler2.Handle(notification, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => handler3.Handle(notification, A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Publish_When_CancellationRequested_Then_DoNotExecuteHandlers()
        {
            // Arrange
            var strategy = new SynchronousStrategy();
            var notification = new TestNotification();
            var handler = A.Fake<INotificationHandler<TestNotification>>();
            var cts = new CancellationTokenSource();
            cts.Cancel();

            A.CallTo(() => handler.Handle(notification, A<CancellationToken>._))
                .Returns(ValueTask.CompletedTask);

            var handlers = new[] { handler };

            // Act
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>await strategy.Publish(notification, handlers, cts.Token));

            // Assert
            A.CallTo(() => handler.Handle(notification, cts.Token)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Publish_When_HandlersWithDelay_Then_AwaitsEachHandler()
        {
            // Arrange
            var strategy = new SynchronousStrategy();
            var notification = new TestNotification();
            var handler1 = A.Fake<INotificationHandler<TestNotification>>();
            var handler2 = A.Fake<INotificationHandler<TestNotification>>();

            A.CallTo(() => handler1.Handle(notification, A<CancellationToken>._))
                .ReturnsLazily(() => new ValueTask(Task.Delay(50)));

            A.CallTo(() => handler2.Handle(notification, A<CancellationToken>._))
                .ReturnsLazily(() => new ValueTask(Task.Delay(50)));

            var handlers = new[] { handler1, handler2 };

            // Act
            var startTime = DateTime.UtcNow;
            await strategy.Publish(notification, handlers, CancellationToken.None);
            var elapsed = DateTime.UtcNow - startTime;

            // Assert - should take at least 100ms (two sequential 50ms delays)
            Assert.True(elapsed.TotalMilliseconds >= 80); // Allow some margin
            A.CallTo(() => handler1.Handle(notification, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => handler2.Handle(notification, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }
    }
}
