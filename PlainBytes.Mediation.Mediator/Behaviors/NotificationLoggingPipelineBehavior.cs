using Microsoft.Extensions.Logging;
using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Behaviors
{
    /// <summary>
    /// Request logging behavior, logs exceptions in pipelines.
    /// </summary>
    public sealed class NotificationLoggingPipelineBehavior<TNotification>(ILogger<NotificationLoggingPipelineBehavior<TNotification>> logger)
        : INotificationBehavior<TNotification> where TNotification : INotification
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async ValueTask Handle(TNotification notification, Func<ValueTask> next, CancellationToken cancellationToken)
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Notification {request} failed", TypeExtensions.GetFormattedName<TNotification>());

                throw; // Propagate the exception
            }
        }
    }
}