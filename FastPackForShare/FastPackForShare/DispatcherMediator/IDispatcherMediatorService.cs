using FastPackForShare.Default;

namespace FastPackForShare.DispatcherMediator;

public interface IDispatcherMediatorService
{
    Task Send<TCommand>(TCommand command) where TCommand : class;
    Task<TResult> Send<TQuery, TResult>(TQuery query) where TQuery : class
                                                      where TResult : BaseDTOModel;
}

