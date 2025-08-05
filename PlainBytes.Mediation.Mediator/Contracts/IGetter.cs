namespace PlainBytes.Mediation.Mediator.Contracts;

/// <summary>
/// Defines the contract for executing query operations that retrieve data.
/// This interface is specifically designed for read-only operations in CQRS patterns.
/// </summary>
/// <remarks>
/// The IGetter interface is used for query operations that:
/// <list type="bullet">
/// <item>Retrieve data without modifying state</item>
/// <item>Always return a typed response</item>
/// <item>Follow CQRS (Command Query Responsibility Segregation) principles</item>
/// </list>
/// This separation from ISender helps distinguish between commands (state-changing) and queries (read-only).
/// </remarks>
/// <example>
/// <code>
/// // Execute a query
/// var users = await getter.Get(new GetAllUsersQuery());
/// var user = await getter.Get(new GetUserByIdQuery { Id = 123 });
/// </code>
/// </example>
public interface IGetter
{
    /// <summary>
    /// Executes a query operation and returns the requested data.
    /// </summary>
    /// <typeparam name="TResponse">The type of data to be returned by the query.</typeparam>
    /// <param name="request">The query object that implements <see cref="IQuery{TResponse}"/>.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask{TResponse}"/> representing the asynchronous operation with the query result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no handler is registered for the query type.</exception>
    ValueTask<TResponse> Get<TResponse>(IQuery<TResponse> request, CancellationToken cancellationToken = default);
}