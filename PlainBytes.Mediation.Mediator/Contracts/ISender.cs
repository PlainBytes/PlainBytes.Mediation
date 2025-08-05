namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Defines the contract for sending requests that expect responses or void operations.
    /// This interface handles command and query operations through the mediator pattern.
    /// </summary>
    /// <remarks>
    /// The ISender interface supports two types of operations:
    /// <list type="bullet">
    /// <item>Request-Response operations that return a typed response</item>
    /// <item>Fire-and-forget operations that don't return a value</item>
    /// </list>
    /// All operations are asynchronous and support cancellation tokens.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Request with response
    /// var result = await sender.Send(new GetUserQuery { Id = 123 });
    /// 
    /// // Request without response
    /// await sender.Send(new DeleteUserCommand { Id = 123 });
    /// </code>
    /// </example>
    public interface ISender
    {
        /// <summary>
        /// Sends a request that expects a typed response.
        /// </summary>
        /// <typeparam name="TResponse">The type of response expected from the request.</typeparam>
        /// <param name="request">The request object that implements <see cref="IRequest{TResponse}"/>.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A <see cref="ValueTask{TResponse}"/> representing the asynchronous operation with the response.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no handler is registered for the request type.</exception>
        ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Sends a request that doesn't expect a response (void operation).
        /// </summary>
        /// <param name="request">The request object that implements <see cref="IRequest"/>.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no handler is registered for the request type.</exception>
        ValueTask Send(IRequest request, CancellationToken cancellationToken = default);
    }
}