using PlainBytes.Mediation.Mediator.Contracts;

namespace SampleApp.Queries;

public class FailingQuery : IQuery<string>;

internal sealed class FailingQueryHandler : IRequestHandler<FailingQuery, string>
{
    public ValueTask<string> Handle(FailingQuery request, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("This query is intentionally failing.");
    }
}