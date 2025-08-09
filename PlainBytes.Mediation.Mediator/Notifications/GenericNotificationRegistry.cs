using System.Collections;
using System.Collections.Concurrent;
using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Notifications
{
    internal sealed class GenericNotificationRegistry<TNotification> : IDisposable, INotificationRegistry<TNotification> where TNotification : INotification
    {
        private sealed class HandlerRegistration(
            GenericNotificationRegistry<TNotification> parent,
            INotificationHandler<TNotification> handler)
            : IDisposable
        {
            private bool _disposed;

            public void Dispose()
            {
                if (!_disposed)
                {
                    parent.Unregister(handler);
                    _disposed = true;
                }
            }
        }

        private readonly ConcurrentDictionary<INotificationHandler<TNotification>, HandlerRegistration> _registry = new();

        public IDisposable Register(INotificationHandler<TNotification> handler)
        {
            ArgumentNullException.ThrowIfNull(handler);
            return _registry.GetOrAdd(handler, h => new HandlerRegistration(this, h));
        }

        private void Unregister(INotificationHandler<TNotification> handler)
        {
            ArgumentNullException.ThrowIfNull(handler);
            _registry.TryRemove(handler, out _);
        }

        public IEnumerator<INotificationHandler<TNotification>> GetEnumerator() => _registry.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            var registrations = _registry.Values.ToArray();

            foreach (var registration in registrations)
            {
                registration.Dispose();
            }
                
            _registry.Clear();
        }
    }
}