using System.Diagnostics;
using Microsoft.Extensions.Logging;
using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Behaviors
{
    /// <summary>
    /// Request logging behavior, logs exceptions and the time the request took in pipelines.
    /// </summary>
    public sealed class RequestPerformanceLoggingPipelineBehavior<TRequest, TResponse>(ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async ValueTask<TResponse> Handle(TRequest request, Func<ValueTask<TResponse>> next, CancellationToken cancellationToken)
        {
            var start = Stopwatch.GetTimestamp();
            var requestName = TypeExtensions.GetFormattedName<TRequest>();

            try
            {
                var result = await next();

                logger.LogInformation("Request {requestName} completed in {time} ms", requestName, Stopwatch.GetElapsedTime(start).TotalMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Request {requestName} failed in {time} ms", requestName, Stopwatch.GetElapsedTime(start).TotalMilliseconds);

                throw; // Propagate the exception
            }
        }
    }
}