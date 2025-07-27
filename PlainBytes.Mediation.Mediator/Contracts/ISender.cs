namespace PlainBytes.Mediation.Mediator.Contracts
{
    public interface ISender
    {
        ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
        ValueTask Send(IRequest request, CancellationToken cancellationToken = default);
    }

    public interface IGetter
    {
        ValueTask<TResponse> Get<TResponse>(IQuery<TResponse> request, CancellationToken cancellationToken = default);
    }
}