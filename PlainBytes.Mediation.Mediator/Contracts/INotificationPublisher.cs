namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Defines the low-level interface for publishing notifications to registered handlers.
    /// This interface provides the core publishing mechanism that can be customized
    /// with different strategies for notification delivery.
    /// </summary>
    /// <remarks>
    /// This interface is typically implemented by:
    /// <list type="bullet">
    /// <item>Sequential notification publishers (handlers executed one after another)</item>
    /// <item>Parallel notification publishers (handlers executed concurrently)</item>
    /// <item>Custom strategy publishers (user-defined execution patterns)</item>
    /// <item>Priority-based publishers (handlers executed by priority)</item>
    /// </list>
    /// The INotificationPublisher is used internally by the mediator and can be customized
    /// to change how notifications are delivered to handlers.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class ParallelNotificationPublisher : INotificationPublisher
    /// {
    ///     public async ValueTask Publish(IEnumerable&lt;INotificationHandler&lt;INotification&gt;&gt; handlers, 
    ///                                    INotification notification, 
    ///                                    CancellationToken cancellationToken)
    ///     {
    ///         var tasks = handlers.Select(handler => handler.Handle(notification, cancellationToken));
    ///         await ValueTask.WhenAll(tasks);
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface INotificationPublisher
    {
        /// <summary>
        /// Publishes a notification to the specified collection of handlers using a specific strategy.
        /// </summary>
        /// <param name="handlers">The collection of notification handlers that should process the notification.</param>
        /// <param name="notification">The notification instance to be published.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous publishing operation.</returns>
        /// <remarks>
        /// Implementations determine how handlers are executed:
        /// <list type="bullet">
        /// <item>Sequential execution - handlers run one after another</item>
        /// <item>Parallel execution - handlers run concurrently</item>
        /// <item>Priority-based execution - handlers run in order of priority</item>
        /// <item>Custom strategies - user-defined execution patterns</item>
        /// </list>
        /// The implementation should handle exceptions appropriately to ensure one failing handler
        /// doesn't prevent others from executing (unless that's the desired behavior).
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="handlers"/> or <paramref name="notification"/> is null.</exception>
        ValueTask Publish(IEnumerable<INotificationHandler<INotification>> handlers, INotification notification, CancellationToken cancellationToken);
    }
}