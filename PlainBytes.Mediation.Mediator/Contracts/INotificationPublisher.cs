namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Defines a publisher that can publish notifications (events).
    /// There must be only on registered publisher.
    /// </summary>
    public interface INotificationPublisher
    {
        Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification;
    }
}