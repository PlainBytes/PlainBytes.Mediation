using System.Collections;
using PlainBytes.Mediation.Mediator.Contracts;
using PlainBytes.Mediation.Mediator.Strategies;

namespace PlainBytes.Mediation.Mediator.Notifications
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
}