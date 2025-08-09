using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Strategies
{
    /// <summary>
    /// Represents a notification publisher strategy that publishes notifications to handlers synchronously.
    /// </summary>
    public class SynchronousStrategy : IPublisherStrategy
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public const string Name = "Synchronous";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async ValueTask Publish<TNotification>(TNotification notification, IEnumerable<INotificationHandler<TNotification>> handlers, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            foreach (var handler in handlers)
            {
                await handler.Handle(notification, cancellationToken);
            }
        }
    }
}