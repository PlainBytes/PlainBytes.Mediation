using PlainBytes.Mediation.Mediator.Contracts;
using PlainBytes.Mediation.Mediator.Notifications;

namespace PlainBytes.Mediation.Mediator.Tests.Notifications
{
    public class NotificationRegistryGenericTests
    {
        public class TestNotification : INotification { }

        [Fact]
        public void Register_AddsHandler_AndDisposeRemovesHandler()
        {
            // Arrange
            var registry = new GenericNotificationRegistry<TestNotification>();
            var handler = A.Fake<INotificationHandler<TestNotification>>();

            // Act
            var registration = registry.Register(handler);

            // Assert
            Assert.Contains(handler, registry);

            // Act
            registration.Dispose();

            // Assert
            Assert.DoesNotContain(handler, registry);
        }

        [Fact]
        public void Dispose_RemovesAllHandlers()
        {
            // Arrange
            var registry = new GenericNotificationRegistry<TestNotification>();
            var handler1 = A.Fake<INotificationHandler<TestNotification>>();
            var handler2 = A.Fake<INotificationHandler<TestNotification>>();
            registry.Register(handler1);
            registry.Register(handler2);

            // Act
            registry.Dispose();

            // Assert
            Assert.Empty(registry);
        }
    }
}