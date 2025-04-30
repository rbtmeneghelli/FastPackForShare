using FastPackForShare.Default;

namespace FastPackForShare.DispatcherMediator;

public interface IDispatcherMediatorQueryHandler<TQuery, TResult> where TQuery : class
                                                                  where TResult : BaseDTOModel
{
    Task<TResult> Handle(TQuery query);
}
