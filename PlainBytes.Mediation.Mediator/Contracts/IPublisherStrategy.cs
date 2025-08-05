namespace PlainBytes.Mediation.Mediator.Contracts;

/// <summary>
/// Defines the contract for implementing custom notification publishing strategies.
/// This interface allows for different approaches to handling multiple notification handlers,
/// such as sequential execution, parallel processing, or custom error handling.
/// </summary>
/// <remarks>
/// The IPublisherStrategy interface enables:
/// <list type="bullet">
/// <item>Custom execution patterns for notification handlers</item>
/// <item>Control over handler execution order and concurrency</item>
/// <item>Implementation of different error handling strategies</item>
/// <item>Performance optimization for specific notification scenarios</item>
/// </list>
/// Strategies are typically used with <see cref="IStrategicNotification"/> implementations
/// to define how their handlers should be executed.
/// </remarks>
public interface IPublisherStrategy
{
    /// <summary>
    /// Publishes a notification to the provided handlers using the strategy's specific execution pattern.
    /// </summary>
    /// <typeparam name="TNotification">The type of notification being published.</typeparam>
    /// <param name="notification">The notification object that implements <see cref="INotification"/>.</param>
    /// <param name="handlers">The collection of handlers that should receive the notification.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous publishing operation.</returns>
    /// <remarks>
    /// The implementation defines how handlers are executed (sequential, parallel, etc.)
    /// and how errors are handled during the notification process.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="notification"/> or <paramref name="handlers"/> is null.</exception>
    ValueTask Publish<TNotification>(TNotification notification, IEnumerable<INotificationHandler<TNotification>> handlers, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}