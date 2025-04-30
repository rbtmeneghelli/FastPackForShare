namespace FastPackForShare.DispatcherMediator;

public interface IDispatcherMediatorCommandHandler<TCommand> where TCommand : class
{
    Task Handle(TCommand command);
}

