using Confluent.Kafka;
using FastPackForShare.Extensions;
using FastPackForShare.Interfaces;
using FastPackForShare.Interfaces.BrokerMessages;
using FastPackForShare.Models;

namespace FastPackForShare.Services.BrokerMessages;

public sealed class KafkaService<TEntity> : IKafkaService<TEntity> where TEntity : class
{
    public KafkaService(INotificationMessageService iNotificationMessageService) : base(iNotificationMessageService)
    {
    }

    public override void SetKafkaSettingByEnvironmentVariables(KafkaModel kafkaModel)
    {
        base.SetKafkaSettingByEnvironmentVariables(kafkaModel);
    }

    public override async Task SendMessageToQueue(string topicName, TEntity entity)
    {
        var producerConfig = _ProducerConfig;
        using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();

        try
        {
            // Nesse exemplo abaixo estamos enviando uma mensagem sem chave
            // Para enviar com chave, basta informar o valor na propriedade Key
            var message = new Message<Null, string>
            {
                Value = entity.SerializeObject()
            };

            var deliveryReport = await producer.ProduceAsync(topicName, message);

            if (deliveryReport is { Status: PersistenceStatus.Persisted })
            {
                Console.WriteLine($"Mensagem entregue com sucesso: {deliveryReport.TopicPartitionOffset}");
            }
            else
            {
                Console.WriteLine($"Falha ao entregar a mensagem");
            }
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Falha ao produzir a mensagem: {e.Error.Reason}");
        }
    }

    public override async Task ReceiveMessageFromQueue(string topicName, string consumerGroup)
    {
        var consumerConfig = GetSetConsumer(consumerGroup);

        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

        consumer.Subscribe(topicName);

        try
        {
            while (true)
            {
                var message = consumer.Consume();
                var objectResult = message.Message.Value.DeserializeObject<TEntity>();
                await Task.CompletedTask;
            }
        }
        catch (OperationCanceledException)
        {
            consumer.Close();
        }
    }
}
