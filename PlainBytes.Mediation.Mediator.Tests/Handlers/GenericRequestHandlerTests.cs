using Microsoft.Extensions.DependencyInjection;
using PlainBytes.Mediation.Mediator.Contracts;
using PlainBytes.Mediation.Mediator.Handlers;

namespace PlainBytes.Mediation.Mediator.Tests.Handlers
{
    public class GenericRequestHandlerTests
    {
        public class TestRequest : IRequest { }

        [Fact]
        public async Task Handle_When_HandlerRegistered_Then_HandlerIsCalled()
        {
            // Arrange
            var handler = A.Fake<IRequestHandler<TestRequest>>();
            var serviceProvider = CreateServiceProvider(handler);
            var sut = new GenericRequestHandler<TestRequest>();
            var request = new TestRequest();

            A.CallTo(() => handler.Handle(request, CancellationToken.None))
                .Returns(ValueTask.CompletedTask);

            // Act
            await sut.Handle(request, serviceProvider, CancellationToken.None);

            // Assert
            A.CallTo(() => handler.Handle(request, CancellationToken.None))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Handle_When_NoBehaviors_Then_OnlyHandlerIsCalled()
        {
            // Arrange
            var handler = A.Fake<IRequestHandler<TestRequest>>();
            var serviceProvider = CreateServiceProvider(handler);
            var sut = new GenericRequestHandler<TestRequest>();
            var request = new TestRequest();

            A.CallTo(() => handler.Handle(request, A<CancellationToken>._))
                .Returns(ValueTask.CompletedTask);

            // Act
            await sut.Handle(request, serviceProvider);

            // Assert
            A.CallTo(() => handler.Handle(request, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Handle_When_SingleBehaviorRegistered_Then_BehaviorIsCalled()
        {
            // Arrange
            var handler = A.Fake<IRequestHandler<TestRequest>>();
            var behavior = A.Fake<IPipelineBehavior<TestRequest, None>>();
            var serviceProvider = CreateServiceProvider(handler, behavior);
            var sut = new GenericRequestHandler<TestRequest>();
            var request = new TestRequest();

            A.CallTo(() => handler.Handle(request, A<CancellationToken>._))
                .Returns(ValueTask.CompletedTask);
            A.CallTo(() => behavior.Handle(request, A<Func<ValueTask<None>>>._, A<CancellationToken>._))
                .ReturnsLazily((TestRequest _, Func<ValueTask<None>> next, CancellationToken _) => next());

            // Act
            await sut.Handle(request, serviceProvider);

            // Assert
            A.CallTo(() => behavior.Handle(request, A<Func<ValueTask<None>>>._, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => handler.Handle(request, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Handle_When_MultipleBehaviorsRegistered_Then_AllBehaviorsAreCalled()
        {
            // Arrange
            var handler = A.Fake<IRequestHandler<TestRequest>>();
            var behavior1 = A.Fake<IPipelineBehavior<TestRequest, None>>();
            var behavior2 = A.Fake<IPipelineBehavior<TestRequest, None>>();
            var behavior3 = A.Fake<IPipelineBehavior<TestRequest, None>>();
            var serviceProvider = CreateServiceProvider(handler, behavior1, behavior2, behavior3);
            var sut = new GenericRequestHandler<TestRequest>();
            var request = new TestRequest();

            A.CallTo(() => handler.Handle(request, A<CancellationToken>._))
                .Returns(ValueTask.CompletedTask);
            A.CallTo(() => behavior1.Handle(request, A<Func<ValueTask<None>>>._, A<CancellationToken>._))
                .ReturnsLazily((TestRequest _, Func<ValueTask<None>> next, CancellationToken _) => next());
            A.CallTo(() => behavior2.Handle(request, A<Func<ValueTask<None>>>._, A<CancellationToken>._))
                .ReturnsLazily((TestRequest _, Func<ValueTask<None>> next, CancellationToken _) => next());
            A.CallTo(() => behavior3.Handle(request, A<Func<ValueTask<None>>>._, A<CancellationToken>._))
                .ReturnsLazily((TestRequest _, Func<ValueTask<None>> next, CancellationToken _) => next());

            // Act
            await sut.Handle(request, serviceProvider);

            // Assert
            A.CallTo(() => behavior1.Handle(request, A<Func<ValueTask<None>>>._, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => behavior2.Handle(request, A<Func<ValueTask<None>>>._, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => behavior3.Handle(request, A<Func<ValueTask<None>>>._, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => handler.Handle(request, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Handle_When_MultipleBehaviors_Then_ExecutedInReverseRegistrationOrder()
        {
            // Arrange
            var executionOrder = new List<string>();
            var handler = A.Fake<IRequestHandler<TestRequest>>();
            var behavior1 = A.Fake<IPipelineBehavior<TestRequest, None>>();
            var behavior2 = A.Fake<IPipelineBehavior<TestRequest, None>>();
            var behavior3 = A.Fake<IPipelineBehavior<TestRequest, None>>();
            var serviceProvider = CreateServiceProvider(handler, behavior1, behavior2, behavior3);
            var sut = new GenericRequestHandler<TestRequest>();
            var request = new TestRequest();

            A.CallTo(() => handler.Handle(request, A<CancellationToken>._))
                .ReturnsLazily(() =>
                {
                    executionOrder.Add("handler");
                    return ValueTask.CompletedTask;
                });

            A.CallTo(() => behavior1.Handle(request, A<Func<ValueTask<None>>>._, A<CancellationToken>._))
                .ReturnsLazily(call =>
                {
                    executionOrder.Add("behavior1-start");
                    var next = call.GetArgument<Func<ValueTask<None>>>(1)!;
                    var result = next().GetAwaiter().GetResult();
                    executionOrder.Add("behavior1-end");
                    return ValueTask.FromResult(result);
                });

            A.CallTo(() => behavior2.Handle(request, A<Func<ValueTask<None>>>._, A<CancellationToken>._))
                .ReturnsLazily(call =>
                {
                    executionOrder.Add("behavior2-start");
                    var next = call.GetArgument<Func<ValueTask<None>>>(1)!;
                    var result = next().GetAwaiter().GetResult();
                    executionOrder.Add("behavior2-end");
                    return ValueTask.FromResult(result);
                });

            A.CallTo(() => behavior3.Handle(request, A<Func<ValueTask<None>>>._, A<CancellationToken>._))
                .ReturnsLazily(call =>
                {
                    executionOrder.Add("behavior3-start");
                    var next = call.GetArgument<Func<ValueTask<None>>>(1)!;
                    var result = next().GetAwaiter().GetResult();
                    executionOrder.Add("behavior3-end");
                    return ValueTask.FromResult(result);
                });

            // Act
            await sut.Handle(request, serviceProvider);

            // Assert - behaviors are reversed in code but DI returns them in registration order, 
            // so execution order is: 1 -> 2 -> 3 -> handler -> 3 -> 2 -> 1
            Assert.Equal(new[]
            {
                "behavior1-start",
                "behavior2-start",
                "behavior3-start",
                "handler",
                "behavior3-end",
                "behavior2-end",
                "behavior1-end"
            }, executionOrder);
        }

        [Fact]
        public async Task Handle_When_RequestIsNull_Then_ThrowsArgumentNullException()
        {
            // Arrange
            var handler = A.Fake<IRequestHandler<TestRequest>>();
            var serviceProvider = CreateServiceProvider(handler);
            var sut = new GenericRequestHandler<TestRequest>();
            object request = null!;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await sut.Handle(request, serviceProvider));

            Assert.Equal("request", exception.ParamName);
        }

        [Fact]
        public async Task Handle_When_ProviderIsNull_Then_ThrowsArgumentNullException()
        {
            // Arrange
            var sut = new GenericRequestHandler<TestRequest>();
            var request = new TestRequest();
            IServiceProvider provider = null!;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await sut.Handle(request, provider));

            Assert.Equal("provider", exception.ParamName);
        }

        [Fact]
        public async Task Handle_When_CancellationTokenProvided_Then_PassedToHandler()
        {
            // Arrange
            var handler = A.Fake<IRequestHandler<TestRequest>>();
            var serviceProvider = CreateServiceProvider(handler);
            var sut = new GenericRequestHandler<TestRequest>();
            var request = new TestRequest();
            var cts = new CancellationTokenSource();

            A.CallTo(() => handler.Handle(request, cts.Token))
                .Returns(ValueTask.CompletedTask);

            // Act
            await sut.Handle(request, serviceProvider, cts.Token);

            // Assert
            A.CallTo(() => handler.Handle(request, cts.Token))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Handle_When_CancellationTokenProvided_Then_PassedToBehaviors()
        {
            // Arrange
            var handler = A.Fake<IRequestHandler<TestRequest>>();
            var behavior = A.Fake<IPipelineBehavior<TestRequest, None>>();
            var serviceProvider = CreateServiceProvider(handler, behavior);
            var sut = new GenericRequestHandler<TestRequest>();
            var request = new TestRequest();
            var cts = new CancellationTokenSource();

            A.CallTo(() => handler.Handle(request, A<CancellationToken>._))
                .Returns(ValueTask.CompletedTask);
            A.CallTo(() => behavior.Handle(request, A<Func<ValueTask<None>>>._, cts.Token))
                .ReturnsLazily((TestRequest _, Func<ValueTask<None>> next, CancellationToken _) => next());

            // Act
            await sut.Handle(request, serviceProvider, cts.Token);

            // Assert
            A.CallTo(() => behavior.Handle(request, A<Func<ValueTask<None>>>._, cts.Token))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Handle_When_HandlerThrowsException_Then_ExceptionPropagates()
        {
            // Arrange
            var handler = A.Fake<IRequestHandler<TestRequest>>();
            var serviceProvider = CreateServiceProvider(handler);
            var sut = new GenericRequestHandler<TestRequest>();
            var request = new TestRequest();
            var expectedException = new InvalidOperationException("Test exception");

            A.CallTo(() => handler.Handle(request, A<CancellationToken>._))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await sut.Handle(request, serviceProvider));

            Assert.Same(expectedException, actualException);
        }

        [Fact]
        public async Task Handle_When_BehaviorThrowsException_Then_ExceptionPropagates()
        {
            // Arrange
            var handler = A.Fake<IRequestHandler<TestRequest>>();
            var behavior = A.Fake<IPipelineBehavior<TestRequest, None>>();
            var serviceProvider = CreateServiceProvider(handler, behavior);
            var sut = new GenericRequestHandler<TestRequest>();
            var request = new TestRequest();
            var expectedException = new InvalidOperationException("Behavior exception");

            A.CallTo(() => behavior.Handle(request, A<Func<ValueTask<None>>>._, A<CancellationToken>._))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await sut.Handle(request, serviceProvider));

            Assert.Same(expectedException, actualException);
        }

        [Fact]
        public async Task Handle_When_BehaviorShortCircuits_Then_HandlerNotCalled()
        {
            // Arrange
            var handler = A.Fake<IRequestHandler<TestRequest>>();
            var behavior = A.Fake<IPipelineBehavior<TestRequest, None>>();
            var serviceProvider = CreateServiceProvider(handler, behavior);
            var sut = new GenericRequestHandler<TestRequest>();
            var request = new TestRequest();

            // Behavior doesn't call next, short-circuiting the pipeline
            A.CallTo(() => behavior.Handle(request, A<Func<ValueTask<None>>>._, A<CancellationToken>._))
                .Returns(ValueTask.FromResult(new None()));

            // Act
            await sut.Handle(request, serviceProvider);

            // Assert
            A.CallTo(() => behavior.Handle(request, A<Func<ValueTask<None>>>._, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => handler.Handle(A<TestRequest>._, A<CancellationToken>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task Handle_When_HandlerReturnsCompletedTask_Then_CompletesSuccessfully()
        {
            // Arrange
            var handler = A.Fake<IRequestHandler<TestRequest>>();
            var serviceProvider = CreateServiceProvider(handler);
            var sut = new GenericRequestHandler<TestRequest>();
            var request = new TestRequest();

            A.CallTo(() => handler.Handle(request, A<CancellationToken>._))
                .Returns(ValueTask.CompletedTask);

            // Act
            var task = sut.Handle(request, serviceProvider);

            // Assert
            Assert.True(task.IsCompleted);
            await task; // Should not throw
        }

        private static IServiceProvider CreateServiceProvider(
            IRequestHandler<TestRequest> handler,
            params IPipelineBehavior<TestRequest, None>[] behaviors)
        {
            var services = new ServiceCollection();
            services.AddSingleton(handler);

            foreach (var behavior in behaviors)
            {
                services.AddSingleton(behavior);
            }

            return services.BuildServiceProvider();
        }
    }
}
