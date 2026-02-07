namespace FastPackForShare.Models;

public readonly record struct RabbitMQConsumerModel
{
    public required string QueueName { get; init; }
    public required bool QueueIsDurable { get; init; }

    public RabbitMQConsumerModel() { }

    public RabbitMQConsumerModel(string queueName, bool queueIsDurable)
    {
        QueueName = queueName;
        QueueIsDurable = queueIsDurable;
    }
}
