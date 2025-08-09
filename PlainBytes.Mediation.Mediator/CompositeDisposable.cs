using System.Diagnostics;

namespace PlainBytes.Mediation.Mediator
{
    /// <summary>
    /// Aggregates multiple IDisposable instances and disposes them together.
    /// </summary>
    internal sealed class CompositeDisposable : IDisposable
    {
        private readonly object _syncRoot = new();
        private List<IDisposable>? _disposables = new();

        public int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    return _disposables?.Count ?? 0;
                }
            }
        }

        /// <summary>
        /// Adds an <see cref="IDisposable"/> instance to the composite.
        /// If the composite has already been disposed, the instance is disposed immediately.
        /// </summary>
        /// <param name="disposable">The disposable instance to add.</param>
        public void Add(IDisposable disposable)
        {
            ArgumentNullException.ThrowIfNull(disposable);

            lock (_syncRoot)
            {
                if (_disposables is null)
                {
                    // Already disposed, dispose immediately
                    disposable.Dispose();
                }
                else
                {
                    _disposables.Add(disposable);
                }
            }
        }

        /// <summary>
        /// Disposes all added <see cref="IDisposable"/> instances and prevents further additions.
        /// Any exceptions during disposal are caught and logged.
        /// </summary>
        public void Dispose()
        {
            List<IDisposable>? toDispose = null;

            lock (_syncRoot)
            {
                if (_disposables is not null)
                {
                    toDispose = _disposables;
                    _disposables = null;
                }
            }

            if (toDispose is not null)
            {
                foreach (var disposable in toDispose)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch(Exception ex)
                    {
                        // Log the exception, but continue disposing other disposables
                        Debug.WriteLine($"Error disposing disposable: {ex.Message}");
                    }
                }
            }
        }
    }
}