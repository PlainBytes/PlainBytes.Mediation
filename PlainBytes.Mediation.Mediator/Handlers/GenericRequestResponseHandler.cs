using Microsoft.Extensions.DependencyInjection;
using PlainBytes.Mediation.Mediator.Contracts;

namespace PlainBytes.Mediation.Mediator.Handlers
{
    internal interface IHandler<TResponse>
    {
        ValueTask<TResponse> Handle(object request, IServiceProvider provider, CancellationToken cancellationToken = default);
    }

    internal sealed class GenericRequestResponseHandler<TRequest, TResponse> : IHandler<TResponse> where TRequest : IRequest<TResponse>
    {
        public ValueTask<TResponse> Handle(object request, IServiceProvider provider, CancellationToken cancellationToken = default)
            => HandleConcrete((TRequest)request, provider, cancellationToken);

        private async ValueTask<TResponse> HandleConcrete(TRequest request, IServiceProvider provider, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(provider);
            
            var handler = provider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
            var behaviors = provider.GetServices<IPipelineBehavior<TRequest, TResponse>>().Reverse();

            var handlerDelegate = () => handler.Handle(request, cancellationToken);

            foreach (var behavior in behaviors)
            {
                var next = handlerDelegate;
                var nextBehavior = behavior;
                handlerDelegate = () => nextBehavior.Handle(request, next, cancellationToken);
            }

            return await handlerDelegate();
        }
    }
}