using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using PlainBytes.Mediation.Mediator;
using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Benchmarks;

[MemoryDiagnoser]
public class MediatorSendBenchmarks
{
    private IMediator _mediator = null!;
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

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddTransient<IRequestHandler<TestCommand>, TestCommandHandler>();
        services.AddTransient<IRequestHandler<TestQuery, string>, TestQueryHandler>();

        var serviceProvider = services.BuildServiceProvider();
        _mediator = serviceProvider.GetRequiredService<IMediator>();

        _command = new TestCommand();
        _query = new TestQuery();
    }

    [Benchmark]
    public async ValueTask Send_Command()
    {
        await _mediator.Send(_command);
    }

    [Benchmark]
    public async ValueTask<string> Get_Query()
    {
        return await _mediator.Get(_query);
    }

    [Benchmark]
    public async ValueTask Send_Command_1K()
    {
        for (int i = 0; i < 1000; i++)
        {
            await _mediator.Send(_command);
        }
    }

    [Benchmark]
    public async ValueTask Get_Query_1K()
    {
        for (int i = 0; i < 1000; i++)
        {
            await _mediator.Get(_query);
        }
    }
}
