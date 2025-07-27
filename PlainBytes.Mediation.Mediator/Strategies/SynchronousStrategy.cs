using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Strategies
{
    public class SynchronousStrategy : IPublisherStrategy
    {
        public const string Name = "Synchronous";

        public async ValueTask Publish<TNotification>(TNotification notification, IEnumerable<INotificationHandler<TNotification>> handlers, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            foreach (var handler in handlers)
            {
                await handler.Handle(notification, cancellationToken);
            }
        }
    }
}