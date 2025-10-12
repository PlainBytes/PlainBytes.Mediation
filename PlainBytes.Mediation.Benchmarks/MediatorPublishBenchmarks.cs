using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using PlainBytes.Mediation.Mediator;
using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Benchmarks;

[MemoryDiagnoser]
public class MediatorPublishBenchmarks
{
    private IMediator _mediator = null!;
    private TestNotification _notification = null!;
    private TestStrategicNotification _synchronousNotification = null!;
    private TestStrategicNotification _parallelNotification = null!;

    public record TestNotification : INotification;
    public record TestStrategicNotification(string Strategy) : IStrategicNotification
    {
        public string StrategyName => Strategy;
    }

    public class TestNotificationHandler1 : INotificationHandler<TestNotification>
    {
        public ValueTask Handle(TestNotification notification, CancellationToken cancellationToken = default)
        {
            return ValueTask.CompletedTask;
        }
    }

    public class TestNotificationHandler2 : INotificationHandler<TestNotification>
    {
        public ValueTask Handle(TestNotification notification, CancellationToken cancellationToken = default)
        {
            return ValueTask.CompletedTask;
        }
    }

    public class TestNotificationHandler3 : INotificationHandler<TestNotification>
    {
        public ValueTask Handle(TestNotification notification, CancellationToken cancellationToken = default)
        {
            return ValueTask.CompletedTask;
        }
    }

    public class TestStrategicNotificationHandler1 : INotificationHandler<TestStrategicNotification>
    {
        public ValueTask Handle(TestStrategicNotification notification, CancellationToken cancellationToken = default)
        {
            return ValueTask.CompletedTask;
        }
    }
    
    public class TestStrategicNotificationHandler2 : INotificationHandler<TestStrategicNotification>
    {
        public ValueTask Handle(TestStrategicNotification notification, CancellationToken cancellationToken = default)
        {
            return ValueTask.CompletedTask;
        }
    }
    
    public class TestStrategicNotificationHandler3 : INotificationHandler<TestStrategicNotification>
    {
        public ValueTask Handle(TestStrategicNotification notification, CancellationToken cancellationToken = default)
        {
            return ValueTask.CompletedTask;
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddTransient<INotificationHandler<TestNotification>, TestNotificationHandler1>();
        services.AddTransient<INotificationHandler<TestNotification>, TestNotificationHandler2>();
        services.AddTransient<INotificationHandler<TestNotification>, TestNotificationHandler3>();
        services.AddTransient<INotificationHandler<TestStrategicNotification>, TestStrategicNotificationHandler1>();
        services.AddTransient<INotificationHandler<TestStrategicNotification>, TestStrategicNotificationHandler2>();
        services.AddTransient<INotificationHandler<TestStrategicNotification>, TestStrategicNotificationHandler3>();

        var serviceProvider = services.BuildServiceProvider();
        _mediator = serviceProvider.GetRequiredService<IMediator>();

        _notification = new TestNotification();
        _synchronousNotification = new TestStrategicNotification("Synchronous");
        _parallelNotification = new TestStrategicNotification("Parallel");
    }

    [Benchmark]
    public async ValueTask Publish_DefaultStrategy_MultipleHandlers()
    {
        for (int i = 0; i < 1000; i++)
        {
            await _mediator.Publish(_notification);
        }
    }

    [Benchmark]
    public async ValueTask Publish_StrategicNotifications_Synchronous()
    {
        for (int i = 0; i < 1000; i++)
        {
            await _mediator.Publish(_synchronousNotification);
        }
    }

    [Benchmark]
    public async ValueTask Publish_StrategicNotifications_Parallel()
    {
        for (int i = 0; i < 1000; i++)
        {
            await _mediator.Publish(_parallelNotification);
        }
    }
}
