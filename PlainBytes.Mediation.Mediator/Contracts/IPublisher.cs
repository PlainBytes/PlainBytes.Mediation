namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Defines the contract for publishing notifications to multiple handlers.
    /// This interface implements the observer pattern through the mediator, allowing
    /// loose coupling between event publishers and subscribers.
    /// </summary>
    /// <remarks>
    /// The IPublisher interface supports:
    /// <list type="bullet">
    /// <item>Publishing notifications to zero or more handlers</item>
    /// <item>Strategic notification publishing with custom strategies</item>
    /// <item>Asynchronous notification processing</item>
    /// <item>Cancellation support for long-running notification processing</item>
    /// </list>
    /// Unlike requests, notifications don't return values and can have multiple handlers.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Publish a simple notification
    /// await publisher.Publish(new UserRegisteredNotification { UserId = 123 });
    /// 
    /// // Publish with cancellation
    /// await publisher.Publish(notification, cancellationToken);
    /// 
    /// // Strategic notification (uses specific strategy)
    /// await publisher.Publish(new HighPriorityNotification { Message = "Alert!" });
    /// </code>
    /// </example>
    public interface IPublisher
    {
        /// <summary>
        /// Publishes a notification to all registered handlers.
        /// </summary>
        /// <param name="notification">The notification object that implements <see cref="INotification"/>.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous publishing operation.</returns>
        /// <remarks>
        /// The notification will be sent to all handlers registered for its type.
        /// If the notification implements <see cref="IStrategicNotification"/>, 
        /// it will be processed using the specified strategy.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="notification"/> is null.</exception>
        ValueTask Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification;
    }
}