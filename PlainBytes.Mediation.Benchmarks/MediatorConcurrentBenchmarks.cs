using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using PlainBytes.Mediation.Mediator;
using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Benchmarks;

[MemoryDiagnoser]
public class MediatorConcurrentBenchmarks
{
    private IMediator _mediator = null!;
    private TestCommand _command = null!;
    private TestQuery _query = null!;
    private TestNotification _notification = null!;

    public record TestCommand : IRequest;
    public record TestQuery : IQuery<string>;
    public record TestNotification : INotification;

    public class TestCommandHandler : IRequestHandler<TestCommand>
    {
        public ValueTask Handle(TestCommand request, CancellationToken cancellationToken = default)
        {
            return ValueTask.CompletedTask;
        }
    }

    public class TestQueryHandler : IRequestHandler<TestQuery, string>
    {
        public ValueTask<string> Handle(TestQuery request, CancellationToken cancellationToken = default)
        {
            return ValueTask.FromResult("result");
        }
    }

    public class TestNotificationHandler : INotificationHandler<TestNotification>
    {
        public ValueTask Handle(TestNotification notification, CancellationToken cancellationToken = default)
        {
            return ValueTask.CompletedTask;
        }
    }

    [Params(1, 10, 100)]
    public int ConcurrencyLevel { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddTransient<IRequestHandler<TestCommand>, TestCommandHandler>();
        services.AddTransient<IRequestHandler<TestQuery, string>, TestQueryHandler>();
        services.AddTransient<INotificationHandler<TestNotification>, TestNotificationHandler>();

        var serviceProvider = services.BuildServiceProvider();
        _mediator = serviceProvider.GetRequiredService<IMediator>();

        _command = new TestCommand();
        _query = new TestQuery();
        _notification = new TestNotification();
    }

    [Benchmark]
    public async Task Concurrent_Send_Commands()
    {
        var tasks = new Task[ConcurrencyLevel];
        for (int i = 0; i < ConcurrencyLevel; i++)
        {
            tasks[i] = _mediator.Send(_command).AsTask();
        }
        await Task.WhenAll(tasks);
    }

    [Benchmark]
    public async Task Concurrent_Get_Queries()
    {
        var tasks = new Task<string>[ConcurrencyLevel];
        for (int i = 0; i < ConcurrencyLevel; i++)
        {
            tasks[i] = _mediator.Get(_query).AsTask();
        }
        await Task.WhenAll(tasks);
    }

    [Benchmark]
    public async Task Concurrent_Publish_Notifications()
    {
        var tasks = new Task[ConcurrencyLevel];
        for (int i = 0; i < ConcurrencyLevel; i++)
        {
            tasks[i] = _mediator.Publish(_notification).AsTask();
        }
        await Task.WhenAll(tasks);
    }

    [Benchmark]
    public async Task Concurrent_Mixed_Operations()
    {
        var tasks = new Task[ConcurrencyLevel * 3];
        for (int i = 0; i < ConcurrencyLevel; i++)
        {
            tasks[i * 3] = _mediator.Send(_command).AsTask();
            tasks[i * 3 + 1] = _mediator.Send(_query).AsTask();
            tasks[i * 3 + 2] = _mediator.Publish(_notification).AsTask();
        }
        await Task.WhenAll(tasks);
    }
}
