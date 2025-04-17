using FastPackForShare.SimpleMediator.Contracts;

namespace FastPackForShare.SimpleMediator;

public record NotificationHandlerExecutor(object HandlerInstance, Func<INotification, CancellationToken, Task> HandlerCallback);
