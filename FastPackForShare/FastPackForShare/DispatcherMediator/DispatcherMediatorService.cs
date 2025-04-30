using FastPackForShare.Default;
using Microsoft.Extensions.DependencyInjection;

namespace FastPackForShare.DispatcherMediator;

public sealed class DispatcherMediatorService : IDispatcherMediatorService
{
    private readonly IServiceProvider _serviceProvider;

    public DispatcherMediatorService(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        _serviceProvider = serviceProvider;
    }

    public async Task Send<TCommand>(TCommand command) where TCommand : class
    {
        var handler = _serviceProvider.GetRequiredService<IDispatcherMediatorCommandHandler<TCommand>>();
        await handler.Handle(command);
    }

    public async Task<TResult> Send<TQuery, TResult>(TQuery query) where TQuery : class
                                                                   where TResult : BaseDTOModel
    {
        var handler = _serviceProvider.GetRequiredService<IDispatcherMediatorQueryHandler<TQuery, TResult>>();
        return await handler.Handle(query);
    }
}
