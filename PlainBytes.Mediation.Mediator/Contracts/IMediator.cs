namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Mediator interface for sending requests and publishing notifications.
    /// </summary>
    public interface IMediator : ISender, IPublisher, IGetter;
}
