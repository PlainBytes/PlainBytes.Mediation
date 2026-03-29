using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlainBytes.Mediation.Mediator.Contracts;
using PlainBytes.Mediation.Mediator.Notifications;

namespace PlainBytes.Mediation.Mediator.Tests.Notifications
{
    public class GenericNotificationHandlerTests
    {
        public class TestNotification : INotification;

        private readonly ServiceCollection _serviceCollection = new();

        public GenericNotificationHandlerTests()
        {
            _serviceCollection.AddMediator();
        }

        private Mediator Create() => new(_serviceCollection.BuildServiceProvider());

        [Fact]
        public async Task Publish_WhenHandlerRegisteredViaDI_ThenHandlerIsCalled()
        {
            // Arrange
            var notification = new TestNotification();
            var handler = A.Fake<INotificationHandler<TestNotification>>();
            _serviceCollection.AddSingleton(handler);
            var sut = Create();

            // Act
            await sut.Publish(notification);

            // Assert
            A.CallTo(() => handler.Handle(notification, CancellationToken.None))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Publish_WhenHandlerRegisteredViaRegistry_ThenHandlerIsCalled()
        {
            // Arrange
            var notification = new TestNotification();
            var handler = A.Fake<INotificationHandler<TestNotification>>();
            var provider = _serviceCollection.BuildServiceProvider();
            var sut = new Mediator(provider);

            var registry = provider.GetRequiredService<INotificationRegistry<TestNotification>>();
            registry.Register(handler);

            // Act
            await sut.Publish(notification);

            // Assert — this handler was registered via the registry, it should still fire
            A.CallTo(() => handler.Handle(notification, CancellationToken.None))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Publish_WhenHandlersRegisteredViaBothDIAndRegistry_ThenAllHandlersAreCalled()
        {
            // Arrange
            var notification = new TestNotification();
            var diHandler = A.Fake<INotificationHandler<TestNotification>>();
            var registryHandler = A.Fake<INotificationHandler<TestNotification>>();

            _serviceCollection.AddSingleton(diHandler);
            var provider = _serviceCollection.BuildServiceProvider();
            var sut = new Mediator(provider);

            var registry = provider.GetRequiredService<INotificationRegistry<TestNotification>>();
            registry.Register(registryHandler);

            // Act
            await sut.Publish(notification);

            // Assert — both DI and registry handlers should fire
            A.CallTo(() => diHandler.Handle(notification, CancellationToken.None))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => registryHandler.Handle(notification, CancellationToken.None))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Publish_WhenRegistryHandlerUnregistered_ThenHandlerIsNotCalled()
        {
            // Arrange
            var notification = new TestNotification();
            var handler = A.Fake<INotificationHandler<TestNotification>>();
            var provider = _serviceCollection.BuildServiceProvider();
            var sut = new Mediator(provider);

            var registry = provider.GetRequiredService<INotificationRegistry<TestNotification>>();
            var registration = registry.Register(handler);
            registration.Dispose(); // unregister before publishing

            // Act
            await sut.Publish(notification);

            // Assert
            A.CallTo(() => handler.Handle(notification, CancellationToken.None))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task Publish_WhenHandlerRegisteredViaNonGenericRegistry_ThenHandlerIsCalled()
        {
            // Arrange
            var notification = new TestNotification();
            var handler = A.Fake<INotificationHandler<TestNotification>>();
            _serviceCollection.AddSingleton(A.Fake<ILogger<NotificationRegistry>>());
            var provider = _serviceCollection.BuildServiceProvider();
            var sut = new Mediator(provider);

            // Register via non-generic INotificationRegistry (reflection-based, requires ILogger)
            var registry = provider.GetRequiredService<INotificationRegistry>();
            registry.Register(handler);

            // Act
            await sut.Publish(notification);

            // Assert — handler registered via non-generic registry should still fire
            A.CallTo(() => handler.Handle(notification, CancellationToken.None))
                .MustHaveHappenedOnceExactly();
        }
    }
}
