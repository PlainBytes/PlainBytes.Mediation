using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using PlainBytes.Mediation.Mediator;
using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Benchmarks;

[MemoryDiagnoser]
public class MediatorPipelineBenchmarks
{
    private IMediator _mediatorNoBehavior = null!;
    private IMediator _mediatorWithOneBehavior = null!;
    private IMediator _mediatorWithThreeBehaviors = null!;
    private TestCommand _command = null!;
    private TestQuery _query = null!;

    public record TestCommand : IRequest;
    public record TestQuery : IQuery<string>;

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

    public class SimplePipelineBehavior1<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async ValueTask<TResponse> Handle(TRequest request, Func<ValueTask<TResponse>> next, CancellationToken cancellationToken = default)
        {
            return await next();
        }
    }

    public class SimplePipelineBehavior2<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async ValueTask<TResponse> Handle(TRequest request, Func<ValueTask<TResponse>> next, CancellationToken cancellationToken = default)
        {
            return await next();
        }
    }

    public class SimplePipelineBehavior3<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async ValueTask<TResponse> Handle(TRequest request, Func<ValueTask<TResponse>> next, CancellationToken cancellationToken = default)
        {
            return await next();
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        // Mediator with no behaviors
        var servicesNoBehavior = new ServiceCollection();
        servicesNoBehavior.AddMediator();
        servicesNoBehavior.AddTransient<IRequestHandler<TestCommand>, TestCommandHandler>();
        servicesNoBehavior.AddTransient<IRequestHandler<TestQuery, string>, TestQueryHandler>();
        _mediatorNoBehavior = servicesNoBehavior.BuildServiceProvider().GetRequiredService<IMediator>();

        // Mediator with one behavior
        var servicesOneBehavior = new ServiceCollection();
        servicesOneBehavior.AddMediator();
        servicesOneBehavior.AddTransient<IRequestHandler<TestCommand>, TestCommandHandler>();
        servicesOneBehavior.AddTransient<IRequestHandler<TestQuery, string>, TestQueryHandler>();
        servicesOneBehavior.AddSingleton(typeof(IPipelineBehavior<,>), typeof(SimplePipelineBehavior1<,>));
        _mediatorWithOneBehavior = servicesOneBehavior.BuildServiceProvider().GetRequiredService<IMediator>();

        // Mediator with three behaviors
        var servicesThreeBehaviors = new ServiceCollection();
        servicesThreeBehaviors.AddMediator();
        servicesThreeBehaviors.AddTransient<IRequestHandler<TestCommand>, TestCommandHandler>();
        servicesThreeBehaviors.AddTransient<IRequestHandler<TestQuery, string>, TestQueryHandler>();
        servicesThreeBehaviors.AddSingleton(typeof(IPipelineBehavior<,>), typeof(SimplePipelineBehavior1<,>));
        servicesThreeBehaviors.AddSingleton(typeof(IPipelineBehavior<,>), typeof(SimplePipelineBehavior2<,>));
        servicesThreeBehaviors.AddSingleton(typeof(IPipelineBehavior<,>), typeof(SimplePipelineBehavior3<,>));
        _mediatorWithThreeBehaviors = servicesThreeBehaviors.BuildServiceProvider().GetRequiredService<IMediator>();

        _command = new TestCommand();
        _query = new TestQuery();
    }

    [Benchmark(Baseline = true)]
    public async ValueTask Send_Command_NoBehavior()
    {
        await _mediatorNoBehavior.Send(_command);
    }

    [Benchmark]
    public async ValueTask Send_Command_OneBehavior()
    {
        await _mediatorWithOneBehavior.Send(_command);
    }

    [Benchmark]
    public async ValueTask Send_Command_ThreeBehaviors()
    {
        await _mediatorWithThreeBehaviors.Send(_command);
    }

    [Benchmark]
    public async ValueTask<string> Get_Query_NoBehavior()
    {
        return await _mediatorNoBehavior.Get(_query);
    }

    [Benchmark]
    public async ValueTask<string> Get_Query_OneBehavior()
    {
        return await _mediatorWithOneBehavior.Get(_query);
    }

    [Benchmark]
    public async ValueTask<string> Get_Query_ThreeBehaviors()
    {
        return await _mediatorWithThreeBehaviors.Get(_query);
    }
}
