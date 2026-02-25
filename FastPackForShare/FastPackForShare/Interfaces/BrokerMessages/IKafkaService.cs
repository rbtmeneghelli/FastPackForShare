using Confluent.Kafka;
using FastPackForShare.Models;
using FastPackForShare.Services.Bases;

namespace FastPackForShare.Interfaces.BrokerMessages;

public abstract class IKafkaService<TEntity> : BaseHandlerService where TEntity : class
{
    public readonly INotificationMessageService _iNotificationMessageService;
    protected ProducerConfig _ProducerConfig { get; private set; }

    protected IKafkaService(INotificationMessageService iNotificationMessageService) : base(iNotificationMessageService)
    {
        _iNotificationMessageService = iNotificationMessageService;
    }

    public virtual void SetKafkaSettingByEnvironmentVariables(KafkaModel kafkaModel) => new ProducerConfig { BootstrapServers = kafkaModel.BootstrapServers };
    protected virtual ConsumerConfig GetSetConsumer(string consumerGroup)
    {
        return new ConsumerConfig
        {
            BootstrapServers = _ProducerConfig.BootstrapServers,
            GroupId = consumerGroup,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }
    public abstract Task SendMessageToQueue(string topicName, TEntity entity);
    public abstract Task ReceiveMessageFromQueue(string topicName, string consumerGroup);
}
