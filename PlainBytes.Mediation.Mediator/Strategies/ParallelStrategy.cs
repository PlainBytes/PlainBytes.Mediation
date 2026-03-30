using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Strategies
{
    /// <summary>
    /// Represents a notification publisher strategy that publishes notifications to multiple handlers in parallel.
    /// </summary>
    public sealed class ParallelStrategy : IPublisherStrategy
    {
        /// <summary>
        /// Defines the name of the strategy
        /// </summary>
        public const string Name = "Parallel";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ValueTask Publish<TNotification>(TNotification notification, IEnumerable<INotificationHandler<TNotification>> handlers, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            ArgumentNullException.ThrowIfNull(handlers);
            ArgumentNullException.ThrowIfNull(notification);

            cancellationToken.ThrowIfCancellationRequested();

            var pendingTasks = new List<Task>();
            List<Exception>? exceptions = null;

            foreach (var handler in handlers)
            {
                try
                {
                    var task = handler.Handle(notification, cancellationToken);
                    if (!task.IsCompletedSuccessfully)
                    {
                        pendingTasks.Add(task.AsTask());
                    }
                }
                catch (Exception ex)
                {
                    exceptions ??= [];
                    exceptions.Add(ex);
                }
            }

            if (pendingTasks.Count == 0 && exceptions is null)
            {
                return ValueTask.CompletedTask;
            }

            return Complete(pendingTasks, exceptions);
        }

        private static async ValueTask Complete(List<Task> pendingTasks, List<Exception>? exceptions)
        {
            if (pendingTasks.Count > 0)
            {
                var whenAll = Task.WhenAll(pendingTasks);

                try
                {
                    await whenAll;
                }
                catch
                {
                    exceptions ??= [];
                    exceptions.AddRange(whenAll.Exception!.InnerExceptions);
                }
            }

            if (exceptions is { Count: 1 })
            {
                throw exceptions[0];
            }

            if (exceptions is { Count: > 0 })
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}
