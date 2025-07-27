namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Handles a notification (event).
    /// </summary>
    /// <typeparam name="TNotification">The type of the notification.</typeparam>
    public interface INotificationHandler<in TNotification> where TNotification : INotification
    {
        Task Handle(TNotification notification, CancellationToken cancellationToken = default);
    }
}
