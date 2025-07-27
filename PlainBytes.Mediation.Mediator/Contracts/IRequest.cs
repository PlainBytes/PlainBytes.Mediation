namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Marker interface for a request without a response (command).
    /// </summary>
    public interface IRequest;

    /// <summary>
    /// Marker interface for a request with a response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public interface IRequest<TResponse> : IRequest;

    public interface IQuery<TResult> : IRequest<TResult>;
}
