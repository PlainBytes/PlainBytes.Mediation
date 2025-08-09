using PlainBytes.Mediation.Mediator.Contracts;

namespace SampleApp.Notifications
{
    internal sealed class SendToastNotificationHandler : INotificationHandler<UserCreatedNotification>
    {
        public ValueTask Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Sending toast notification to: {notification.UserName}");
            return ValueTask.CompletedTask;
        }
    }
}