using Microsoft.Extensions.Logging;
using PlainBytes.Mediation.Mediator.Contracts;
using System.Diagnostics;

namespace PlainBytes.Mediation.Mediator.Behaviors
{
    /// <summary>
    /// Request logging behavior, logs exceptions in pipelines.
    /// </summary>
    public sealed class NotificationPerformanceLoggingPipelineBehavior<TNotification>(ILogger<NotificationLoggingPipelineBehavior<TNotification>> logger)
        : INotificationBehavior<TNotification> where TNotification : INotification
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async ValueTask Handle(TNotification notification, Func<ValueTask> next, CancellationToken cancellationToken)
        {
            var start = Stopwatch.GetTimestamp();
            var notificationName = TypeExtensions.GetFormattedName<TNotification>();

            try
            {
                await next();

                logger.LogInformation("Notification {notification} published in {time} ms", notificationName, Stopwatch.GetElapsedTime(start).TotalMilliseconds);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Notification {notification} publishing failed in {time} ms", notificationName, Stopwatch.GetElapsedTime(start).TotalMilliseconds);

                throw; // Propagate the exception
            }
        }
    }
}