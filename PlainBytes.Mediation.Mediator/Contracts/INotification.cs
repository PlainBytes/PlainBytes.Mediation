namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Marker interface for a notification (event).
    /// </summary>
    public interface INotification;

    public interface IStrategicNotification : INotification
    {
        /// <summary>
        /// Gets the name of the strategy to use for publishing the notification.
        /// </summary>
        string StrategyName { get; }
    }
}
