using Microsoft.Extensions.DependencyInjection;
using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Handlers
{
    internal sealed class GenericNotificationHandler<TNotification> : IHandler where TNotification : INotification
    {
        public ValueTask Handle(object request, IServiceProvider provider, CancellationToken cancellationToken = default)
            => HandleConcrete((TNotification)request, provider, cancellationToken);

        private async ValueTask HandleConcrete(TNotification notification, IServiceProvider provider, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(notification);
            ArgumentNullException.ThrowIfNull(provider);
            
            IPublisherStrategy strategy;

            if (notification is IStrategicNotification strategicNotification)
            {
                strategy = provider.GetRequiredKeyedService<IPublisherStrategy>(strategicNotification.StrategyName);
            }
            else
            {
                strategy = provider.GetRequiredService<IPublisherStrategy>();
            }

            var handlers = provider.GetRequiredService<IEnumerable<INotificationHandler<TNotification>>>();
            var behaviors = provider.GetServices<INotificationBehavior<TNotification>>().Reverse();

            var handlerDelegate = () => strategy.Publish(notification, handlers, cancellationToken);

            foreach (var behavior in behaviors)
            {
                var next = handlerDelegate;
                var nextBehavior = behavior;
                handlerDelegate = async () => await nextBehavior.Handle(notification, next, cancellationToken);
            }

            await handlerDelegate();
        }
    }
}