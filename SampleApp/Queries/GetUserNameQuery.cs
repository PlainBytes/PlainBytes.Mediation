using PlainBytes.Mediation.Mediator.Contracts;

namespace SampleApp.Queries;

public class GetUserNameQuery : IQuery<string>
{
    public int UserId { get; set; }
}

internal sealed class GetUserNameQueryHandler : IRequestHandler<GetUserNameQuery, string>
{
    public ValueTask<string> Handle(GetUserNameQuery request, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult($"User_{request.UserId}");
    }
}