namespace FastPackForShare.Models;

public readonly record struct RabbitMQConsumerModel
{
    public string QueueName { get; init; }
    public bool QueueIsDurable { get; init; }

    public RabbitMQConsumerModel() { }

    public RabbitMQConsumerModel(string queueName, bool queueIsDurable)
    {
        QueueName = queueName;
        QueueIsDurable = queueIsDurable;
    }
}
