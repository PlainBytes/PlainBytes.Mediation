using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Strategies
{
    /// <summary>
    /// Represents a notification publisher strategy that publishes notifications to multiple handlers in parallel.
    /// </summary>
    public sealed class ParallelStrategy : IPublisherStrategy
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public const string Name = "Parallel";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ValueTask Publish<TNotification>(TNotification notification, IEnumerable<INotificationHandler<TNotification>> handlers, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            var tasks = handlers.Select(handler => handler.Handle(notification, cancellationToken))
                .Where(x => x.IsCompletedSuccessfully is false)
                .Select(x => x.AsTask());

            return new ValueTask(Task.WhenAll(tasks));
        }
    }
}
