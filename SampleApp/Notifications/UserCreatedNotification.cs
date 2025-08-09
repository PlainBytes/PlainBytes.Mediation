using PlainBytes.Mediation.Mediator.Contracts;

namespace SampleApp.Notifications;

public class UserCreatedNotification : INotification
{
    public string UserName { get; set; } = string.Empty;
}