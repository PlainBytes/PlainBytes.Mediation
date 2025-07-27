using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using PlainBytes.Mediation.Mediator.Contracts;
using PlainBytes.Mediation.Mediator.Strategies;

namespace PlainBytes.Mediation.Mediator
{
    public class NotificationPublisherStrategies : IEnumerable<KeyValuePair<string, Type>>
    {
        private readonly Dictionary<string, Type> _strategies = new();

        public void AddStrategy<TStrategy>(string name) where TStrategy : IPublisherStrategy
        {
            _strategies[name] = typeof(TStrategy);
        }

        public static NotificationPublisherStrategies GetDefault()
        {
            var strategies = new NotificationPublisherStrategies();

            strategies.AddStrategy<SynchronousStrategy>(SynchronousStrategy.Name);
            strategies.AddStrategy<ParallelStrategy>(ParallelStrategy.Name);

            return strategies;
        }

        public IEnumerator<KeyValuePair<string, Type>> GetEnumerator() => _strategies.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class ServiceCollectionExtensions
    {
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