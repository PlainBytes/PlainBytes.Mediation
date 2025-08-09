using PlainBytes.Mediation.Mediator.Contracts;

namespace SampleApp.Notifications
{
    internal sealed class SendWelcomeEmailNotificationHandler : INotificationHandler<UserCreatedNotification>
    {
        public ValueTask Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Sending welcome email to: {notification.UserName}");
            return ValueTask.CompletedTask;
        }
    }
}