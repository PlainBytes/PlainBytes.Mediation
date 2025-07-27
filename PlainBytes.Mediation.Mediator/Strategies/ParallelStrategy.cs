using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Strategies
{
    public sealed class ParallelStrategy : IPublisherStrategy
    {
        public const string Name = "Parallel";

        public ValueTask Publish<TNotification>(TNotification notification, IEnumerable<INotificationHandler<TNotification>> handlers, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            var tasks = handlers.Select(handler => handler.Handle(notification, cancellationToken));

            return new ValueTask(Task.WhenAll(tasks));
        }
    }
}
