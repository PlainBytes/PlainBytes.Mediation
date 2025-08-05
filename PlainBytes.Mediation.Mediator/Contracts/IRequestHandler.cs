namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Handles a request without a response (command).
    /// </summary>
    /// <remarks>
    /// This interface is typically implemented by command handlers that perform operations
    /// without returning a value. Commands represent state-changing operations in the
    /// application and follow the Command pattern through the mediator.
    /// </remarks>
    /// <typeparam name="TRequest">The type of the request that implements <see cref="IRequest"/>.</typeparam>
    /// <example>
    /// <code>
    /// public class DeleteUserCommandHandler : IRequestHandler&lt;DeleteUserCommand&gt;
    /// {
    ///     public async ValueTask Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    ///     {
    ///         // Implementation logic here
    ///         await _userRepository.DeleteAsync(request.UserId, cancellationToken);
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IRequestHandler<in TRequest> where TRequest : IRequest
    {
        /// <summary>
        /// Handles the specified request asynchronously.
        /// </summary>
        /// <param name="request">The request to handle.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        ValueTask Handle(TRequest request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Handles a request and returns a response.
    /// </summary>
    /// <remarks>
    /// This interface is typically implemented by query handlers that retrieve data
    /// and return a typed response. Queries represent read-only operations in the
    /// application and follow the Query pattern through the mediator.
    /// </remarks>
    /// <typeparam name="TRequest">The type of the request that implements <see cref="IRequest{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
    /// <example>
    /// <code>
    /// public class GetUserQueryHandler : IRequestHandler&lt;GetUserQuery, User&gt;
    /// {
    ///     public async ValueTask&lt;User&gt; Handle(GetUserQuery request, CancellationToken cancellationToken)
    ///     {
    ///         // Implementation logic here
    ///         return await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Handles the specified request asynchronously and returns a response.
        /// </summary>
        /// <param name="request">The request to handle.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A <see cref="ValueTask{TResponse}"/> representing the asynchronous operation with the response.</returns>
        ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
    }
}