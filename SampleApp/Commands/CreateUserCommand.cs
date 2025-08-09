using PlainBytes.Mediation.Mediator.Contracts;

namespace SampleApp.Commands;

public class CreateUserCommand : IRequest
{
    public string UserName { get; set; } = string.Empty;
}

internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
{
    public ValueTask Handle(CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"User '{request.UserName}' created.");
        return ValueTask.CompletedTask;
    }
}