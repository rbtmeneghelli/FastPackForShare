using FastPackForShare.Interfaces;
using FastPackForShare.Interfaces.BrokerMessages;
using FastPackForShare.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FastPackForShare.Services.BrokerMessages;

public sealed class RabbitMQService<T> : IRabbitMQService<T> where T : class
{
    public RabbitMQService(INotificationMessageService iNotificationMessageService) : base(iNotificationMessageService)
    {
    }

    public override void SetRabbitMQSettingByEnvironmentVariables(RabbitMQModel rabbitMQModel)
    {
        base.SetRabbitMQSettingByEnvironmentVariables(rabbitMQModel);
    }

    /// <summary>
    /// Publisher FanOut to send same message from queues in sameTime. ExchangeName must be the same in SendandReceive
    /// </summary>
    /// <param name="ExchangeName"></param>
    /// <param name="QueueName"></param>
    /// <param name="ObjectValue"></param>
    /// <returns></returns>
    public override async Task SendMessageToQueueInSameTime(string ExchangeName, T ObjectValue)
    {
        using var connection = await _ConnectionFactory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Fanout);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ObjectValue));
        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(
                    exchange: ExchangeName,
                    routingKey: "",
                    mandatory: false,
                    basicProperties: properties,
                    body: new ReadOnlyMemory<byte>(body));
        Console.WriteLine($"Enviado mensagem {body} para o publicador {ExchangeName} com sucesso");
    }

    /// <summary>
    /// Publisher FanOut to send same message from queues in sameTime. ExchangeName must be the same in SendandReceive
    /// </summary>
    /// <param name="ExchangeName"></param>
    /// <param name="QueueName"></param>
    /// <param name="ObjectValue"></param>
    /// <returns></returns>
    public override async Task ReceiveMessageToQueueInSameTime(string ExchangeName)
    {
        using var connection = await _ConnectionFactory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Fanout);

        #region [Will generate Random Queue]
        var queueName = channel.QueueDeclareAsync().GetAwaiter().GetResult().QueueName;
        #endregion

        await channel.QueueBindAsync(queue: queueName,
                          exchange: ExchangeName,
                          routingKey: "");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            //await _utilService.ProcessingTask(message);
        };

        await channel.BasicConsumeAsync(queue: queueName,
                             autoAck: true,
                             consumer: consumer);

        await Task.CompletedTask;
    }

    /// <summary>
    /// Publisher Direct to send message from queues with same routing key and exchangeName. 
    /// </summary>
    /// <param name="ExchangeName"></param>
    /// <param name="RoutingKey"></param>
    /// <param name="ObjectValue"></param>
    /// <returns></returns>
    public override async Task SendMessageToQueueRouting(string ExchangeName, string RoutingKey, T ObjectValue)
    {
        using var connection = await _ConnectionFactory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Direct);
        await channel.QueueDeclareAsync(queue: "Excel",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ObjectValue));
        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(
                     exchange: ExchangeName,
                     routingKey: "Excel",
                     mandatory: false,
                     basicProperties: properties,
                     body: body);
        Console.WriteLine($"Enviado mensagem {JsonSerializer.Serialize(ObjectValue)} para o publicador {ExchangeName} com destino a rota {RoutingKey} com sucesso");
        await Task.CompletedTask;
    }

    /// <summary>
    /// Publisher Direct to send message from queues with same routing key and exchangeName. 
    /// </summary>
    /// <param name="ExchangeName"></param>
    /// <param name="RoutingKey"></param>
    /// <param name="ObjectValue"></param>
    /// <returns></returns>
    public override async Task ReceiveMessageToQueueRouting(string ExchangeName, string RoutingKey)
    {
        using var connection = await _ConnectionFactory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Direct);
        var queueName = channel.QueueDeclareAsync().GetAwaiter().GetResult().QueueName;
        await channel.QueueBindAsync(queue: queueName, exchange: ExchangeName, routingKey: RoutingKey);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            //await _utilService.ProcessingTask(message);
        };

        await channel.BasicConsumeAsync(queue: queueName,
                             autoAck: true,
                             consumer: consumer);

        await Task.CompletedTask;
    }
}
