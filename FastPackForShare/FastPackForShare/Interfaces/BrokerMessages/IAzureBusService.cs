using Azure.Messaging.ServiceBus;
using FastPackForShare.Models;
using FastPackForShare.Services.Bases;

namespace FastPackForShare.Interfaces.BrokerMessages;

public abstract class IAzureBusService<TEntity> : BaseHandlerService, IAsyncDisposable where TEntity : class
{
    public readonly INotificationMessageService _iNotificationMessageService;
    protected ServiceBusClient _ServiceBusClient { get; private set; }
    protected ServiceBusProcessor _processor { get; set; }

    protected IAzureBusService(INotificationMessageService iNotificationMessageService) : base(iNotificationMessageService)
    {
        _iNotificationMessageService = iNotificationMessageService;
    }

    public virtual void SetServiceBusSettingByEnvironmentVariables(ServiceBusSettings serviceBusSettings) => _ServiceBusClient = new ServiceBusClient(serviceBusSettings.Server);
    public abstract Task SendMessage(string queueName, TEntity entity);
    public abstract Task ReceiveMessage(string queueName, TEntity entity);
    public abstract Task ReceiveMessageAsync(string queueName, TEntity entity);
    public abstract Task SendMessage(string topicName, TEntity entity, CancellationToken cancellationToken = default);

    public async ValueTask DisposeAsync()
    {
        await _processor.DisposeAsync();
    }
}
