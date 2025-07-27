namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Handles a request without a response (command).
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    public interface IRequestHandler<in TRequest> where TRequest : IRequest
    {
        ValueTask Handle(TRequest request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Handles a request and returns a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
    }
}
