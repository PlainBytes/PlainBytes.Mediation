using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Results
{
    /// <summary>
    /// Defines convenience extension methods for <see cref="IMediator"/>.
    /// </summary>
    public static class RequestResultExtensions
    {
        /// <summary>
        /// Creates an instance of <see cref="RequestResult{T}"/> from the provided exception.
        /// </summary>
        /// <typeparam name="T">Type of the wrapped value.</typeparam>
        /// <param name="exception">Exception which prevented returning the expected value.</param>
        /// <returns>Instance of <see cref="RequestResult{T}"/> in error state.</returns>
        public static RequestResult<T> FromException<T>(this Exception exception) =>
            RequestResult<T>.Failure(exception);

        /// <summary>
        /// Creates an instance of <see cref="RequestResult{T}"/> from the provided value.
        /// </summary>
        /// <typeparam name="T">Type of the wrapped value.</typeparam>
        /// <param name="result">The value to wrap.</param>
        /// <returns>Instance of <see cref="RequestResult{T}"/> in success state.</returns>
        public static RequestResult<T> ToResult<T>(this T result) =>
            RequestResult<T>.Successful(result);

        /// <summary>
        /// Attempts to execute the provided command and wraps the outcome into a <see cref="RequestResult{T}"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="mediator">The mediator instance.</param>
        /// <param name="command">The command to send.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>Wrapped result.</returns>
        public static async ValueTask<RequestResult<TResult>> TrySend<TResult>(
            this IMediator mediator,
            IRequest<TResult> command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(mediator);
            ArgumentNullException.ThrowIfNull(command);

            try
            {
                var result = await mediator.Send(command, cancellationToken).ConfigureAwait(false);
                return result.ToResult();
            }
            catch (Exception e)
            {
                return e.FromException<TResult>();
            }
        }

        /// <summary>
        /// Attempts to execute the provided command and wraps the outcome into a <see cref="RequestResult{T}"/>.
        /// </summary>
        /// <param name="mediator">The mediator instance.</param>
        /// <param name="command">The command to send.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>Wrapped result.</returns>
        public static async ValueTask<RequestResult<bool>> TrySend(
            this IMediator mediator,
            IRequest command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(mediator);
            ArgumentNullException.ThrowIfNull(command);

            try
            {
                await mediator.Send(command, cancellationToken).ConfigureAwait(false);
                return true.ToResult();
            }
            catch (Exception e)
            {
                return e.FromException<bool>();
            }
        }

        /// <summary>
        /// Attempts to execute the provided query and wraps the outcome into a <see cref="RequestResult{T}"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="mediator">The mediator instance.</param>
        /// <param name="query">The query to execute.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>Wrapped result.</returns>
        public static async ValueTask<RequestResult<TResult>> TryGet<TResult>(
            this IMediator mediator,
            IQuery<TResult> query,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(mediator);
            ArgumentNullException.ThrowIfNull(query);

            try
            {
                var result = await mediator.Get(query, cancellationToken).ConfigureAwait(false);
                return result.ToResult();
            }
            catch (Exception e)
            {
                return e.FromException<TResult>();
            }
        }
    }
}