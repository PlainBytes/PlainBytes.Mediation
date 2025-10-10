using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Tests;
public class TestNotification : INotification;

internal class HandlersFactory
{
    public static IEnumerable<KeyValuePair<INotificationHandler<TestNotification>, TaskCompletionSource>> CreateNotifications(int numberOfHandlers = 3)
    {
        for (var i = 0; i < numberOfHandlers; i++)
        {
            var taskCompletionSource = new TaskCompletionSource();
            var handler = A.Fake<INotificationHandler<TestNotification>>();
            A.CallTo(() => handler.Handle(A<TestNotification>._, A<CancellationToken>._)).Returns(new ValueTask(taskCompletionSource.Task));
                
            yield return new KeyValuePair<INotificationHandler<TestNotification>, TaskCompletionSource>(A.Fake<INotificationHandler<TestNotification>>(), taskCompletionSource);
        }
    }
}