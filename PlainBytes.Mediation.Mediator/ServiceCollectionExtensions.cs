using Microsoft.Extensions.DependencyInjection;
using PlainBytes.Mediation.Mediator.Behaviors;
using PlainBytes.Mediation.Mediator.Contracts;
using PlainBytes.Mediation.Mediator.Notifications;

namespace PlainBytes.Mediation.Mediator
{
    /// <summary>
    /// Defines extension methods for adding mediator services to the service collection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds mediator services to the service collection.
        /// </summary>
        /// <param name="services">Service collection to which it should be added.</param>
        /// <param name="strategies">Notification publisher strategies to use. If not provided default strategies will be used.</param>
        public static IServiceCollection AddMediator(this IServiceCollection services, NotificationPublisherStrategies? strategies = null)
        {
            ArgumentNullException.ThrowIfNull(services);

            return services
                .AddSingleton<IMediator, Mediator>()
                .AddSingleton<ISender>(x => x.GetRequiredService<IMediator>())
                .AddSingleton<IGetter>(x => x.GetRequiredService<IMediator>())
                .AddSingleton<IPublisher>(x => x.GetRequiredService<IMediator>())
                .AddSingleton<INotificationRegistry, NotificationRegistry>()
                .AddSingleton(typeof(INotificationRegistry<>),typeof(GenericNotificationRegistry<>))
                .AddPublishers(strategies ?? NotificationPublisherStrategies.GetDefault());
        }

        /// <summary>
        /// Adds logging pipeline behaviors to the service collection.
        /// </summary>
        public static IServiceCollection AddLoggingPipelineBehaviors(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);
            return services
                .AddSingleton(typeof(IPipelineBehavior<,>), typeof(RequestLoggingPipelineBehavior<,>))
                .AddSingleton(typeof(INotificationBehavior<>), typeof(NotificationLoggingPipelineBehavior<>));
        }

        /// <summary>
        /// Adds logging pipeline behaviors to the service collection.
        /// </summary>
        public static IServiceCollection AddPerformanceLoggingPipelineBehaviors(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);
            return services
                .AddSingleton(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceLoggingPipelineBehavior<,>))
                .AddSingleton(typeof(INotificationBehavior<>), typeof(NotificationLoggingPipelineBehavior<>));
        }

        internal static IServiceCollection AddPublishers(this IServiceCollection services, NotificationPublisherStrategies strategies)
        {
            bool defaultStrategyRegistered = false;

            foreach (var strategy in strategies)
            {
                if (defaultStrategyRegistered is false)
                {
                    services.AddSingleton(typeof(IPublisherStrategy), strategy.Value);
                    defaultStrategyRegistered = true;
                }

                services.AddKeyedSingleton(typeof(IPublisherStrategy), strategy.Key, strategy.Value);
            }

            return services;
        }
    }
}