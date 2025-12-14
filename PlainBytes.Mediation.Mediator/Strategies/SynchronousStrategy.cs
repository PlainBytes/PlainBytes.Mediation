using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Strategies
{
    /// <summary>
    /// Represents a notification publisher strategy that publishes notifications to handlers synchronously.
    /// </summary>
    public class SynchronousStrategy : IPublisherStrategy
    {
        /// <summary>
        /// Defines the name of the strategy
        /// </summary>
        public const string Name = "Synchronous";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async ValueTask Publish<TNotification>(TNotification notification, IEnumerable<INotificationHandler<TNotification>> handlers, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            ArgumentNullException.ThrowIfNull(handlers);
            ArgumentNullException.ThrowIfNull(notification);
            
            foreach (var handler in handlers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                await handler.Handle(notification, cancellationToken);
            }
        }
    }
}