namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Defines a behavior that can be executed as part of the notification handling pipeline.
    /// This interface allows for cross-cutting concerns such as logging, validation, or authorization
    /// to be applied to notification processing through a middleware-like pattern.
    /// </summary>
    /// <typeparam name="TNotification">The type of notification that this behavior can handle. Must implement <see cref="INotification"/>.</typeparam>
    /// <remarks>
    /// Notification behaviors are executed in a pipeline pattern where each behavior can:
    /// <list type="bullet">
    /// <item>Perform pre-processing logic before the notification is handled</item>
    /// <item>Call the next behavior in the pipeline using the <paramref name="next"/> delegate</item>
    /// <item>Perform post-processing logic after the notification has been handled</item>
    /// <item>Short-circuit the pipeline by not calling <paramref name="next"/></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class LoggingNotificationBehavior&lt;TNotification&gt; : INotificationBehavior&lt;TNotification&gt;
    ///     where TNotification : INotification
    /// {
    ///     public async ValueTask Handle(TNotification notification, CancellationToken cancellationToken, Func&lt;ValueTask&gt; next)
    ///     {
    ///         // Pre-processing
    ///         Console.WriteLine($"Handling notification: {typeof(TNotification).Name}");
    ///         
    ///         // Execute next behavior in pipeline
    ///         await next();
    ///         
    ///         // Post-processing
    ///         Console.WriteLine($"Finished handling notification: {typeof(TNotification).Name}");
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface INotificationBehavior<in TNotification> where TNotification : INotification
    {
        /// <summary>
        /// Handles the notification within the behavior pipeline.
        /// </summary>
        /// <param name="notification">The notification instance to be processed.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="next">A delegate representing the next behavior in the pipeline. Call this to continue pipeline execution.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// Implementations should:
        /// <list type="bullet">
        /// <item>Perform any required pre-processing logic</item>
        /// <item>Call <paramref name="next"/> to continue the pipeline (unless intentionally short-circuiting)</item>
        /// <item>Perform any required post-processing logic</item>
        /// <item>Handle exceptions appropriately</item>
        /// <item>Respect the <paramref name="cancellationToken"/> for cancellation support</item>
        /// </list>
        /// </remarks>
        ValueTask Handle(TNotification notification, CancellationToken cancellationToken, Func<ValueTask> next);
    }
}