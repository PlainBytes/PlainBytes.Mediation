using Microsoft.Extensions.DependencyInjection;
using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Handlers
{
    internal interface IHandler
    {
        ValueTask Handle(object request, IServiceProvider provider, CancellationToken cancellationToken = default);
    }

    internal sealed class GenericRequestHandler<TRequest> : IHandler where TRequest : IRequest
    {
        public ValueTask Handle(object request, IServiceProvider provider, CancellationToken cancellationToken = default)
            => HandleConcrete((TRequest)request, provider, cancellationToken);

        private async ValueTask HandleConcrete(IRequest request, IServiceProvider provider, CancellationToken cancellationToken = default)
        {
            var handler = provider.GetRequiredService<IRequestHandler<TRequest>>();
            var behaviors = provider.GetServices<IPipelineBehavior<TRequest, None>>().Reverse();

            var handlerDelegate = () => handler.Handle((TRequest)request, cancellationToken);

            foreach (var behavior in behaviors)
            {
                var next = handlerDelegate;
                handlerDelegate = async () => await behavior.Handle((TRequest)request, ToNone(next), cancellationToken);
            }

            await handlerDelegate();
        }

        private static Func<ValueTask<None>> ToNone(Func<ValueTask> next)
        {
            return async () =>
            {
                await next();
                return default;
            };
        }

    }
}