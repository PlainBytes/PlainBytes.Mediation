using Microsoft.Extensions.Logging;
using PlainBytes.Mediation.Mediator.Contracts;
using PlainBytes.Mediation.Mediator.Notifications;

namespace PlainBytes.Mediation.Mediator.Tests.Notifications
{
    public class NotificationRegistryTests
    {
        public class TestNotification1 : INotification;
        public class TestNotification2 : INotification;

        public class TestHandler1 : INotificationHandler<TestNotification1>
        {
            public ValueTask Handle(TestNotification1 notification, CancellationToken cancellationToken) => ValueTask.CompletedTask;
        }

        public class TestHandlerBoth : INotificationHandler<TestNotification1>, INotificationHandler<TestNotification2>
        {
            public ValueTask Handle(TestNotification1 notification, CancellationToken cancellationToken) => ValueTask.CompletedTask;

            public ValueTask Handle(TestNotification2 notification, CancellationToken cancellationToken) => ValueTask.CompletedTask;
        }

        public class TestHandlerNone;

        private readonly IServiceProvider _serviceProvider;
        private readonly NotificationRegistry _sut;

        public NotificationRegistryTests()
        {
            _serviceProvider = A.Fake<IServiceProvider>();

            _sut = new NotificationRegistry(_serviceProvider, A.Fake<ILogger<NotificationRegistry>>());
        }

        [Fact]
        public void Register_GivenOneHandler_RegistersSuccessfully()
        {
            // Arrange
            var handler = new TestHandler1();
            var registry = A.Fake<INotificationRegistry<TestNotification1>>();
            var subscription = A.Fake<IDisposable>();

            A.CallTo(() => _serviceProvider.GetService(typeof(INotificationRegistry<TestNotification1>))).Returns(registry);
            A.CallTo(() => registry.Register(handler)).Returns(subscription);

            // Act
            var result = _sut.Register(handler);

            // Assert
            Assert.IsType<CompositeDisposable>(result);
            var composite = Assert.IsType<CompositeDisposable>(result);
            Assert.Equal(1, composite.Count);

            A.CallTo(() => _serviceProvider.GetService(typeof(INotificationRegistry<TestNotification1>))).MustHaveHappenedOnceExactly();
            A.CallTo(() => registry.Register(handler)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Register_GivenMultipleHandlers_RegistersSuccessfully()
        {
            // Arrange
            var handler = new TestHandlerBoth();
            var fakeRegistry1 = A.Fake<INotificationRegistry<TestNotification1>>();
            var fakeRegistry2 = A.Fake<INotificationRegistry<TestNotification2>>();
            var fakeSubscription1 = A.Fake<IDisposable>();
            var fakeSubscription2 = A.Fake<IDisposable>();

            A.CallTo(() => _serviceProvider.GetService(typeof(INotificationRegistry<TestNotification1>))).Returns(fakeRegistry1);
            A.CallTo(() => fakeRegistry1.Register(handler)).Returns(fakeSubscription1);

            A.CallTo(() => _serviceProvider.GetService(typeof(INotificationRegistry<TestNotification2>))).Returns(fakeRegistry2);
            A.CallTo(() => fakeRegistry2.Register(handler)).Returns(fakeSubscription2);

            // Act
            var result = _sut.Register(handler);

            // Assert
            Assert.IsType<CompositeDisposable>(result);
            var composite = Assert.IsType<CompositeDisposable>(result);
            Assert.Equal(2, composite.Count);

            A.CallTo(() => _serviceProvider.GetService(typeof(INotificationRegistry<TestNotification1>))).MustHaveHappenedOnceExactly();
            A.CallTo(() => _serviceProvider.GetService(typeof(INotificationRegistry<TestNotification2>))).MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeRegistry1.Register(handler)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRegistry2.Register(handler)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Register_GivenNoHandlers_ReturnsEmptyCompositeDisposable()
        {
            // Arrange
            var handler = new TestHandlerNone();

            // Act
            var result = _sut.Register(handler);

            // Assert
            Assert.IsType<CompositeDisposable>(result);
            var composite = Assert.IsType<CompositeDisposable>(result);
            Assert.Equal(0, composite.Count);

            A.CallTo(_serviceProvider).MustNotHaveHappened();
        }

        [Fact]
        public void Register_GivenServiceIsNotFound_ThrowsException()
        {
            // Arrange
            var handler = new TestHandler1();
            var expectedException = new InvalidOperationException("Service not found");

            A.CallTo(() => _serviceProvider.GetService(typeof(INotificationRegistry<TestNotification1>)))
                .Throws(expectedException);

            // Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _sut.Register(handler));
            Assert.Same(expectedException, ex);

            A.CallTo(() => _serviceProvider.GetService(typeof(INotificationRegistry<TestNotification1>)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Register_WhenRegistryReturnsNull_ThrowsInvalidCastException()
        {
            // Arrange
            var handler = new TestHandler1();
            var registry = A.Fake<INotificationRegistry<TestNotification1>>();

            A.CallTo(() => _serviceProvider.GetService(typeof(INotificationRegistry<TestNotification1>))).Returns(registry);
            A.CallTo(() => registry.Register(handler)).Returns(null!);

            // Assert
            Assert.Throws<InvalidCastException>(() => _sut.Register(handler));

            A.CallTo(() => _serviceProvider.GetService(typeof(INotificationRegistry<TestNotification1>)))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => registry.Register(handler)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Register_WhenRegistryThrowsException_ThrowsException()
        {
            // Arrange
            var handler = new TestHandler1();
            var registry = A.Fake<INotificationRegistry<TestNotification1>>();
            var expectedException = new ApplicationException("Registration failed");

            A.CallTo(() => _serviceProvider.GetService(typeof(INotificationRegistry<TestNotification1>))).Returns(registry);
            A.CallTo(() => registry.Register(handler)).Throws(expectedException);

            // Assert
            var ex = Assert.Throws<ApplicationException>(() => _sut.Register(handler));
            Assert.Same(expectedException, ex);

            A.CallTo(() => _serviceProvider.GetService(typeof(INotificationRegistry<TestNotification1>))).MustHaveHappenedOnceExactly();
            A.CallTo(() => registry.Register(handler)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Register_HandlerWithMultipleNotifications_FirstRegistrationFails_ThrowsException()
        {
            // Arrange
            var handler = new TestHandlerBoth();
            var fakeRegistry1 = A.Fake<INotificationRegistry<TestNotification1>>();
            var fakeRegistry2 = A.Fake<INotificationRegistry<TestNotification2>>();
            var expectedException = new InvalidOperationException("First registration failed");

            A.CallTo(() => _serviceProvider.GetService(typeof(INotificationRegistry<TestNotification1>))).Returns(fakeRegistry1);
            A.CallTo(() => _serviceProvider.GetService(typeof(INotificationRegistry<TestNotification2>))).Returns(fakeRegistry2);

            A.CallTo(() => fakeRegistry1.Register(handler)).Throws(expectedException);
            A.CallTo(() => fakeRegistry2.Register(handler)).Returns(A.Fake<IDisposable>());

            // Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _sut.Register(handler));
            Assert.Same(expectedException, ex);

            A.CallTo(() => _serviceProvider.GetService(typeof(INotificationRegistry<TestNotification1>))).MustHaveHappenedOnceExactly();
            A.CallTo(() => _serviceProvider.GetService(typeof(INotificationRegistry<TestNotification2>))).MustNotHaveHappened();

            A.CallTo(() => fakeRegistry1.Register(handler)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRegistry2.Register(handler)).MustNotHaveHappened();
        }
    }
}