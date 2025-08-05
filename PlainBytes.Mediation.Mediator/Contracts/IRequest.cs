namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Marker interface for requests that don't return a response.
    /// These are typically used for command operations that perform actions without returning data.
    /// </summary>
    /// <remarks>
    /// This interface is used for:
    /// <list type="bullet">
    /// <item>Command operations (Create, Update, Delete)</item>
    /// <item>Fire-and-forget operations</item>
    /// <item>Operations where only success/failure matters</item>
    /// </list>
    /// For operations that need to return data, use <see cref="IRequest{TResponse}"/> instead.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class DeleteUserCommand : IRequest
    /// {
    ///     public int UserId { get; set; }
    /// }
    /// 
    /// // Usage
    /// await mediator.Send(new DeleteUserCommand { UserId = 123 });
    /// </code>
    /// </example>
    public interface IRequest;

    /// <summary>
    /// Defines a request that expects a typed response.
    /// These are typically used for both command operations that return data and query operations.
    /// </summary>
    /// <typeparam name="TResponse">The type of response that will be returned by this request.</typeparam>
    /// <remarks>
    /// This interface is used for:
    /// <list type="bullet">
    /// <item>Query operations (Read, Search, Get)</item>
    /// <item>Command operations that return data (Create with ID, Update with result)</item>
    /// <item>Any operation where the caller needs a response</item>
    /// </list>
    /// For operations that don't need to return data, use <see cref="IRequest"/> instead.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class CreateUserCommand : IRequest&lt;UserDto&gt;
    /// {
    ///     public string Name { get; set; }
    ///     public string Email { get; set; }
    /// }
    /// 
    /// public class GetUserQuery : IRequest&lt;UserDto&gt;
    /// {
    ///     public int UserId { get; set; }
    /// }
    /// 
    /// // Usage
    /// var newUser = await mediator.Send(new CreateUserCommand { Name = "John", Email = "john@example.com" });
    /// var existingUser = await mediator.Send(new GetUserQuery { UserId = 123 });
    /// </code>
    /// </example>
    public interface IRequest<out TResponse> : IRequest;

    /// <summary>
    /// Marker interface for query operations that return typed responses.
    /// This interface is specifically designed for read-only operations in CQRS patterns.
    /// </summary>
    /// <typeparam name="TResult">The type of data that will be returned by this query.</typeparam>
    /// <remarks>
    /// This interface is used for:
    /// <list type="bullet">
    /// <item>Read-only operations that don't modify state</item>
    /// <item>Data retrieval operations</item>
    /// <item>Search and filter operations</item>
    /// <item>CQRS query-side operations</item>
    /// </list>
    /// The separation from <see cref="IRequest{TResponse}"/> helps distinguish between
    /// commands (state-changing) and queries (read-only) in CQRS architectures.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class GetAllUsersQuery : IQuery&lt;List&lt;UserDto&gt;&gt;
    /// {
    ///     public int PageSize { get; set; } = 10;
    ///     public int PageNumber { get; set; } = 1;
    /// }
    /// 
    /// public class SearchUsersQuery : IQuery&lt;List&lt;UserDto&gt;&gt;
    /// {
    ///     public string SearchTerm { get; set; }
    /// }
    /// 
    /// // Usage
    /// var allUsers = await mediator.Get(new GetAllUsersQuery());
    /// var searchResults = await mediator.Get(new SearchUsersQuery { SearchTerm = "john" });
    /// </code>
    /// </example>
    public interface IQuery<out TResult> : IRequest<TResult>;
}