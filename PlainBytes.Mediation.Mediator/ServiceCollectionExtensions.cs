using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using PlainBytes.Mediation.Mediator.Contracts;
using PlainBytes.Mediation.Mediator.Strategies;

namespace PlainBytes.Mediation.Mediator
{
    /// <summary>
    /// Represents a collection of notification publisher strategies.
    /// </summary>
    public class NotificationPublisherStrategies : IEnumerable<KeyValuePair<string, Type>>
    {
        private readonly Dictionary<string, Type> _strategies = new();

        /// <summary>
        /// Adds a notification publisher strategy to the collection.
        /// </summary>
        /// <typeparam name="TStrategy">Type of the strategy to add.</typeparam>
        /// <param name="name">Name of the strategy.</param>
        public void AddStrategy<TStrategy>(string name) where TStrategy : IPublisherStrategy
        {
            _strategies[name] = typeof(TStrategy);
        }

        /// <summary>
        /// Gets the default notification publisher strategies.
        /// </summary>
        public static NotificationPublisherStrategies GetDefault()
        {
            var strategies = new NotificationPublisherStrategies();

            strategies.AddStrategy<SynchronousStrategy>(SynchronousStrategy.Name);
            strategies.AddStrategy<ParallelStrategy>(ParallelStrategy.Name);

            return strategies;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerator<KeyValuePair<string, Type>> GetEnumerator() => _strategies.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

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
        public static IServiceCollection AddMediator(this IServiceCollection services, NotificationPublisherStrategies? strategies)
        {
            ArgumentNullException.ThrowIfNull(services);

            return services
                .AddSingleton<IMediator, Mediator>()
                .AddSingleton<ISender>(x => x.GetRequiredService<IMediator>())
                .AddSingleton<IGetter>(x => x.GetRequiredService<IMediator>())
                .AddSingleton<IPublisher>(x => x.GetRequiredService<IMediator>())
                .AddPublishers(strategies ?? NotificationPublisherStrategies.GetDefault());
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