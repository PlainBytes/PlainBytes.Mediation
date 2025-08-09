using PlainBytes.Mediation.Mediator.Contracts;

namespace SampleApp.Commands;

public class CreateUserWithIdCommand : IRequest<int>
{
    public string UserName { get; set; } = string.Empty;
}

internal sealed class CreateUserWithIdCommandHandler : IRequestHandler<CreateUserWithIdCommand, int>
{
    public ValueTask<int> Handle(CreateUserWithIdCommand request, CancellationToken cancellationToken = default)
    {
        var newId = new Random().Next(1000, 9999);

        Console.WriteLine($"User with id '{newId}' created.");

        return ValueTask.FromResult(newId);
    }
}