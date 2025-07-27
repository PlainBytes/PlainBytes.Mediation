namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Defines a pipeline behavior for requests and responses in the mediation flow.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public interface IPipelineBehavior<in TRequest, TResponse> where TRequest : IRequest
    {
        ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<ValueTask<TResponse>> next);
    }
}
