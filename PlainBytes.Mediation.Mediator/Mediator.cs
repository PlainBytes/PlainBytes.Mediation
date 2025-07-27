using PlainBytes.Mediation.Mediator.Contracts;
using PlainBytes.Mediation.Mediator.Handlers;
using System.Collections.Concurrent;

namespace PlainBytes.Mediation.Mediator
{
    internal sealed class Mediator(IServiceProvider serviceProvider) : IMediator
    {
        private static readonly ConcurrentDictionary<Type, object> GenericHandlersCache = new();
        private static readonly ConcurrentDictionary<Type, object> HandlersCache = new();
        private static readonly ConcurrentDictionary<Type, object> NotificationHandlersCache = new();

        public ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var handler = (IHandler<TResponse>)GenericHandlersCache.GetOrAdd(request.GetType(), static type =>
            {
                var handlerType = typeof(GenericRequestResponseHandler<,>).MakeGenericType(type, typeof(TResponse));

                return Activator.CreateInstance(handlerType) ?? throw new InvalidOperationException($"Can not create handler {handlerType} for notification {type}");
            });

            return handler.Handle(request, serviceProvider, cancellationToken);
        }

        public async ValueTask Send(IRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var handler = (IHandler)HandlersCache.GetOrAdd(request.GetType(), static type =>
            {
                var handlerType = typeof(GenericRequestHandler<>).MakeGenericType(type);

                return Activator.CreateInstance(handlerType) ?? throw new InvalidOperationException($"Can not create handler {handlerType} for notification {type}");
            });

            await handler.Handle(request, serviceProvider, cancellationToken);
        }

        public ValueTask Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            ArgumentNullException.ThrowIfNull(notification);

            var handler = (IHandler)NotificationHandlersCache.GetOrAdd(notification.GetType(), static type =>
            {
                var handlerType = typeof(GenericNotificationHandler<>).MakeGenericType(type);

                return Activator.CreateInstance(handlerType) ?? throw new InvalidOperationException($"Can not create handler {handlerType} for notification {type}");
            });

            return handler.Handle(notification, serviceProvider, cancellationToken);
        }

        public ValueTask<TResponse> Get<TResponse>(IQuery<TResponse> request, CancellationToken cancellationToken = default) => Send(request, cancellationToken);
    }
}
