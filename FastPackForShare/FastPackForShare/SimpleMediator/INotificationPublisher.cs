using FastPackForShare.SimpleMediator.Contracts;

namespace FastPackForShare.SimpleMediator;

public interface INotificationPublisher
{
    Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification,
        CancellationToken cancellationToken);
}
