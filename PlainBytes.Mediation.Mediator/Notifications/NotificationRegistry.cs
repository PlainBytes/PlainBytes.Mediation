using Microsoft.Extensions.DependencyInjection;
using PlainBytes.Mediation.Mediator.Contracts;
using System.Reflection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace PlainBytes.Mediation.Mediator.Notifications
{
    internal sealed class NotificationRegistry(IServiceProvider serviceProvider, ILogger<NotificationRegistry> logger)
        : INotificationRegistry
    {
        private static readonly ConcurrentDictionary<Type, (Type RegistryType, MethodInfo RegisterMethod)>
            Cache = new();

        public IDisposable Register(object handlerInstance)
        {
            ArgumentNullException.ThrowIfNull(handlerInstance);

            var handlerType = handlerInstance.GetType();
            var notificationHandlerInterfaces = handlerType.GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(INotificationHandler<>));

            var subscriptions = new CompositeDisposable();

            try
            {
                foreach (var handlerInterface in notificationHandlerInterfaces)
                {
                    var notificationType = handlerInterface.GetGenericArguments()[0];

                    var (registryType, registerMethod) = Cache.GetOrAdd(notificationType, t =>
                    {
                        var regType = typeof(INotificationRegistry<>).MakeGenericType(t);
                        var regMethod = regType.GetMethod(nameof(INotificationRegistry<INotification>.Register))!;
                        return (regType, regMethod);
                    });

                    try
                    {
                        var registryInstance = serviceProvider.GetRequiredService(registryType);
                        var registration = registerMethod.Invoke(registryInstance, [handlerInstance]);

                        if (registration is IDisposable subscription)
                        {
                            subscriptions.Add(subscription);
                        }
                        else
                        {
                            throw new InvalidCastException(
                                "Returned registration value is not a valid IDisposable type");
                        }
                    }
                    catch (TargetInvocationException exception)
                    {
                        logger.LogError(exception,
                            "Error registering handler {name} for notification type {notification}", handlerType.Name,
                            notificationType.Name);
                        if (exception.InnerException is not null)
                        {
                            throw exception.InnerException;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error registering handler {name} for notification type {notification}",
                            handlerType.Name, notificationType.Name);
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                subscriptions.Dispose(); // We failed clean up if there is any registered
                throw;
            }

            return subscriptions;
        }
    }
}