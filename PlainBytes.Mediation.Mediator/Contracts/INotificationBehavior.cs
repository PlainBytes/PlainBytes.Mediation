namespace PlainBytes.Mediation.Mediator.Contracts
{
    public interface INotificationBehavior<in TNotification> where TNotification : INotification
    {
        ValueTask Handle(TNotification notification, CancellationToken cancellationToken, Func<ValueTask> next);
    }
}