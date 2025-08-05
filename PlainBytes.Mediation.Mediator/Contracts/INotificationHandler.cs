namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Defines a handler for processing specific types of notifications.
    /// Multiple handlers can be registered for the same notification type, allowing
    /// for the observer pattern implementation through the mediator.
    /// </summary>
    /// <typeparam name="TNotification">The type of notification that this handler can process. Must implement <see cref="INotification"/>.</typeparam>
    /// <remarks>
    /// Notification handlers are used to:
    /// <list type="bullet">
    /// <item>React to domain events and notifications</item>
    /// <item>Implement side effects (logging, email notifications, cache updates)</item>
    /// <item>Handle cross-cutting concerns triggered by notifications</item>
    /// <item>Process events in an asynchronous, decoupled manner</item>
    /// </list>
    /// Unlike request handlers, multiple notification handlers can be registered for the same notification type,
    /// and they will all be executed when the notification is published.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class EmailNotificationHandler : INotificationHandler&lt;UserRegisteredNotification&gt;
    /// {
    ///     private readonly IEmailService _emailService;
    /// 
    ///     public EmailNotificationHandler(IEmailService emailService)
    ///     {
    ///         _emailService = emailService;
    ///     }
    /// 
    ///     public async ValueTask Handle(UserRegisteredNotification notification, CancellationToken cancellationToken)
    ///     {
    ///         await _emailService.SendWelcomeEmail(notification.UserEmail, cancellationToken);
    ///     }
    /// }
    /// 
    /// public class LoggingNotificationHandler : INotificationHandler&lt;UserRegisteredNotification&gt;
    /// {
    ///     private readonly ILogger _logger;
    /// 
    ///     public LoggingNotificationHandler(ILogger logger)
    ///     {
    ///         _logger = logger;
    ///     }
    /// 
    ///     public async ValueTask Handle(UserRegisteredNotification notification, CancellationToken cancellationToken)
    ///     {
    ///         _logger.LogInformation("User {UserId} registered successfully", notification.UserId);
    ///         await ValueTask.CompletedTask;
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface INotificationHandler<in TNotification> where TNotification : INotification
    {
        /// <summary>
        /// Handles the specified notification asynchronously.
        /// </summary>
        /// <param name="notification">The notification instance to be processed.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous handling operation.</returns>
        /// <remarks>
        /// Implementations should:
        /// <list type="bullet">
        /// <item>Process the notification efficiently and asynchronously</item>
        /// <item>Handle exceptions gracefully to avoid affecting other handlers</item>
        /// <item>Respect the cancellation token for cancellation support</item>
        /// <item>Avoid blocking operations unless necessary</item>
        /// <item>Log any relevant information for debugging and monitoring</item>
        /// </list>
        /// </remarks>
        ValueTask Handle(TNotification notification, CancellationToken cancellationToken);
    }
}