using Microsoft.Extensions.Logging;
using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Behaviors
{
    /// <summary>
    /// Request logging behavior, logs exceptions in pipelines.
    /// </summary>
    public sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async ValueTask<TResponse> Handle(TRequest request, Func<ValueTask<TResponse>> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                var requestName = TypeExtensions.GetFormattedName<TRequest>();

                logger.LogError(ex, "Request {request} failed", requestName);

                throw; // Propagate the exception
            }
        }
    }
}
