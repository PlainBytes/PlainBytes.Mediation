namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Publisher interface for publishing notifications.
    /// </summary>
    public interface IPublisher
    {
        ValueTask Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification;
    }

    public interface IPublisherStrategy
    {
        ValueTask Publish<TNotification>(TNotification notification, IEnumerable<INotificationHandler<TNotification>> handlers, CancellationToken cancellationToken = default)
            where TNotification : INotification;
    }
}