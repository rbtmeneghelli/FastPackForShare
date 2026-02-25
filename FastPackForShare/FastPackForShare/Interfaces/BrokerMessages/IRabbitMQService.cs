using FastPackForShare.Models;
using FastPackForShare.Services.Bases;
using RabbitMQ.Client;

namespace FastPackForShare.Interfaces.BrokerMessages
{
    public abstract class IRabbitMQService<T> : BaseHandlerService where T : class
    {
        public ConnectionFactory _ConnectionFactory { get; private set; }
        public readonly INotificationMessageService _iNotificationMessageService;

        protected IRabbitMQService(INotificationMessageService iNotificationMessageService) : base(iNotificationMessageService)
        {
            _iNotificationMessageService = iNotificationMessageService;
        }

        public virtual void SetRabbitMQSettingByEnvironmentVariables(RabbitMQModel rabbitMQModel)
        {
            _ConnectionFactory = new ConnectionFactory
            {
                HostName = rabbitMQModel.HostName,
                UserName = rabbitMQModel.UserName,
                Password = rabbitMQModel.Password
            };
        }

        public abstract Task SendMessageToQueueInSameTime(string ExchangeName, T ObjectValue);

        public abstract Task ReceiveMessageToQueueInSameTime(string ExchangeName);

        public abstract Task SendMessageToQueueRouting(string ExchangeName, string RoutingKey, T ObjectValue);

        public abstract Task ReceiveMessageToQueueRouting(string ExchangeName, string RoutingKey);
    }
}
