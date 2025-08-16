namespace PlainBytes.Mediation.Mediator.Handlers
{
    internal interface IHandler
    {
        ValueTask Handle(object request, IServiceProvider provider, CancellationToken cancellationToken = default);
    }
}