using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainBytes.Mediation.Mediator.Contracts
{
    /// <summary>
    /// Allows the registration of handles for a specific notification.
    /// Handlers are normally instantiated by when a request is executed,
    /// however in the case of the singleton, scoped classes this is not desired,
    /// therefor instances that have a longer live cycle can register them self for notifications.
    /// </summary>
    /// <typeparam name="TNotification">Type of the notification.</typeparam>
    public interface INotificationRegistry<TNotification> : IEnumerable<INotificationHandler<TNotification>> where TNotification : INotification
    {
        /// <summary>
        /// Registers the provided handler for the specified notification type.
        /// </summary>
        /// <param name="handler">Handler that should be called upon a notification is published.</param>
        /// <returns>A disposable object, when disposed the handler will be removed from the registry.</returns>
        IDisposable Register(INotificationHandler<TNotification> handler);
    }

    /// <summary>
    /// Allows the registration of notification handlers.
    /// Handlers are normally instantiated by when a request is executed,
    /// however in the case of the UI classes this is not desired,
    /// therefor instances that have a longer live cycle can register them self for notifications.
    /// </summary>
    public interface INotificationRegistry
    {
        /// <summary>
        /// Registers the provided handler for all the implemented notifications (<see cref="INotificationHandler{TNotification}"/>).
        /// </summary>
        /// <param name="handler">Handler that should be called upon a notification is published.</param>
        /// <returns>A disposable object, when disposed the handler will be removed from the registry.</returns>
        IDisposable Register(object handler);
    }
}
