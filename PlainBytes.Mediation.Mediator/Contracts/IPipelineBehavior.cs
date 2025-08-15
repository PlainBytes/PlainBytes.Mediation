namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Defines a behavior that can be executed as part of the request processing pipeline.
    /// This interface allows for cross-cutting concerns such as validation, logging, caching,
    /// authorization, and performance monitoring to be applied to request processing
    /// through a middleware-like pattern.
    /// </summary>
    /// <typeparam name="TRequest">The type of request that this behavior can handle. Must implement <see cref="IRequest"/>.</typeparam>
    /// <typeparam name="TResponse">The type of response that will be returned by the request processing.</typeparam>
    /// <example>
    /// <code>
    /// public class ValidationBehavior&lt;TRequest, TResponse&gt; : IPipelineBehavior&lt;TRequest, TResponse&gt;
    ///     where TRequest : IRequest
    /// {
    ///     private readonly IValidator&lt;TRequest&gt; _validator;
    /// 
    ///     public ValidationBehavior(IValidator&lt;TRequest&gt; validator)
    ///     {
    ///         _validator = validator;
    ///     }
    /// 
    ///     public async ValueTask&lt;TResponse&gt; Handle(TRequest request, CancellationToken cancellationToken, 
    ///                                              Func&lt;ValueTask&lt;TResponse&gt;&gt; next)
    ///     {
    ///         // Pre-processing: Validate request
    ///         var validationResult = await _validator.ValidateAsync(request, cancellationToken);
    ///         if (!validationResult.IsValid)
    ///         {
    ///             throw new ValidationException(validationResult.Errors);
    ///         }
    /// 
    ///         // Execute next behavior in pipeline
    ///         var response = await next();
    /// 
    ///         // Post-processing: Log successful completion
    ///         Console.WriteLine($"Request {typeof(TRequest).Name} completed successfully");
    /// 
    ///         return response;
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IPipelineBehavior<in TRequest, TResponse> where TRequest : IRequest
    {
        /// <summary>
        /// Handles the request within the pipeline, allowing for pre and post-processing logic.
        /// </summary>
        /// <param name="request">The request instance to be processed.</param>
        /// <param name="next">A delegate representing the next behavior in the pipeline. Call this to continue pipeline execution.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A <see cref="ValueTask{TResponse}"/> representing the asynchronous operation with the response.</returns>
        /// <remarks>
        /// Implementations should:
        /// <list type="bullet">
        /// <item>Perform any required pre-processing logic</item>
        /// <item>Call <paramref name="next"/> to continue the pipeline (unless intentionally short-circuiting)</item>
        /// <item>Perform any required post-processing logic on the response</item>
        /// <item>Handle exceptions appropriately</item>
        /// <item>Respect the <paramref name="cancellationToken"/> for cancellation support</item>
        /// <item>Return the response (potentially modified) to the previous behavior in the chain</item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
        ValueTask<TResponse> Handle(TRequest request, Func<ValueTask<TResponse>> next, CancellationToken cancellationToken);
    }
}