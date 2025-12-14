using Microsoft.Extensions.DependencyInjection;
using PlainBytes.Mediation.Mediator.Contracts;
using PlainBytes.Mediation.Mediator.Handlers;
using PlainBytes.Mediation.Mediator.Notifications;

namespace PlainBytes.Mediation.Mediator.Tests
{
    public class MediatorTests
    {
        public class TestCommand : IRequest;
        public class TestQuery : IQuery<string>;
        public class TestNotification : INotification;
        public class TestStrategicNotification : IStrategicNotification { public string StrategyName { get; set; } = "Test"; }

        private readonly ServiceCollection _serviceCollection = new();
        private Mediator _sut;

        private IPipelineBehavior<TestCommand, None> CommandBehavior { get; } = A.Fake<IPipelineBehavior<TestCommand, None>>();
        private IRequestHandler<TestCommand> CommandHandler { get; } = A.Fake<IRequestHandler<TestCommand>>();
        private IRequestHandler<TestQuery, string> QueryHandler { get; } = A.Fake<IRequestHandler<TestQuery, string>>();

        public MediatorTests()
        {
            _serviceCollection.AddPublishers(NotificationPublisherStrategies.GetDefault());
            _serviceCollection.AddSingleton(_ => CommandHandler);
            _serviceCollection.AddSingleton(_ => QueryHandler);

            _sut = Create();
        }

        private Mediator Create() => new(_serviceCollection.BuildServiceProvider());

        [Fact]
        public async Task Send_Command_When_HandlerRegistered_Then_HandlerIsCalled()
        {
            // Arrange
            var command = new TestCommand();
            A.CallTo(() => CommandHandler.Handle(command, CancellationToken.None)).Returns(ValueTask.CompletedTask);

            // Act
            await _sut.Send(command);

            // Assert
            A.CallTo(() => CommandHandler.Handle(command, CancellationToken.None)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Send_Command_When_BehaviorRegistered_Then_BehaviorIsCalled()
        {
            // Arrange
            var command = new TestCommand();
            _serviceCollection.AddSingleton(CommandBehavior);
            _sut = Create();

            A.CallTo(() => CommandBehavior.Handle(command, A<Func<ValueTask<None>>>.Ignored, CancellationToken.None)).Returns(ValueTask.FromResult(new None()));

            // Act
            await _sut.Send(command);

            // Assert
            A.CallTo(() => CommandBehavior.Handle(command, A<Func<ValueTask<None>>>.Ignored, CancellationToken.None)).MustHaveHappened();
        }

        [Fact]
        public async Task Send_Command_When_Null_Then_ThrowsArgumentNullException()
        {
            // Arrange
            IRequest command = null!;

            // Act & Assert
            var ex = await Record.ExceptionAsync(async () => await _sut.Send(command));
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public async Task Send_Query_When_HandlerRegistered_Then_HandlerIsCalled()
        {
            // Arrange
            var query = new TestQuery();
            A.CallTo(() => QueryHandler.Handle(query, CancellationToken.None)).Returns(new ValueTask<string>("result"));

            // Act
            var result = await _sut.Get(query);

            // Assert
            Assert.Equal("result", result);
            A.CallTo(() => QueryHandler.Handle(query, CancellationToken.None)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Send_Query_When_Null_Then_ThrowsArgumentNullException()
        {
            // Arrange
            IRequest<string> query = null!;

            // Act & Assert
            var ex = await Record.ExceptionAsync(async () => await _sut.Send(query));
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public async Task Get_Query_When_HandlerRegistered_Then_HandlerIsCalled()
        {
            // Arrange
            var query = new TestQuery();
            A.CallTo(() => QueryHandler.Handle(query, CancellationToken.None)).Returns(new ValueTask<string>("result"));

            // Act
            var result = await _sut.Get(query);

            // Assert
            Assert.Equal("result", result);
            A.CallTo(() => QueryHandler.Handle(query, CancellationToken.None)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Publish_Notification_When_HandlerRegistered_Then_HandlerIsCalled()
        {
            // Arrange
            var notification = new TestNotification();
            var notificationHandler = A.Fake<INotificationHandler<TestNotification>>();
            _serviceCollection.AddSingleton(notificationHandler);

            _sut = Create();

            // Act
            await _sut.Publish(notification);

            // Assert
            A.CallTo(() => notificationHandler.Handle(notification, CancellationToken.None)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Publish_Notification_When_Null_Then_ThrowsArgumentNullException()
        {
            // Arrange
            TestNotification notification = null!;

            // Act & Assert
            var ex = await Record.ExceptionAsync(async () => await _sut.Publish(notification));
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public async Task Publish_StrategicNotification_When_StrategyRegistered_Then_StrategyIsUsed()
        {
            // Arrange
            var notification = new TestStrategicNotification();
            var notificationHandler = A.Fake<INotificationHandler<TestStrategicNotification>>();
            var publisherStrategy = A.Fake<IPublisherStrategy>();
            _serviceCollection.AddSingleton(notificationHandler);
            _serviceCollection.AddKeyedSingleton(notification.StrategyName, publisherStrategy);
            _sut = Create();

            A.CallTo(() => publisherStrategy.Publish(notification, A<IEnumerable<INotificationHandler<TestStrategicNotification>>>._, CancellationToken.None)).Returns(ValueTask.CompletedTask);

            // Act
            await _sut.Publish(notification);

            // Assert
            A.CallTo(() => publisherStrategy.Publish(notification, A<IEnumerable<INotificationHandler<TestStrategicNotification>>>._, CancellationToken.None)).MustHaveHappened();
        }
    }
}
