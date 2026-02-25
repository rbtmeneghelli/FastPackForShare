using Azure.Messaging.ServiceBus;
using FastPackForShare.Interfaces;
using FastPackForShare.Interfaces.BrokerMessages;
using FastPackForShare.Models;

namespace FastPackForShare.Services.BrokerMessages;

public sealed class AzureBusService<TEntity> : IAzureBusService<TEntity> where TEntity : class
{
    public AzureBusService(INotificationMessageService iNotificationMessageService) : base(iNotificationMessageService) { }

    public override void SetServiceBusSettingByEnvironmentVariables(ServiceBusSettings serviceBusSettings)
    {
        base.SetServiceBusSettingByEnvironmentVariables(serviceBusSettings);
    }

    public override async Task SendMessage(string queueName, TEntity entity)
    {
        await using (var client = _ServiceBusClient)
        {
            ServiceBusSender sender = client.CreateSender(queueName);
            ServiceBusMessage message = new ServiceBusMessage(JsonSerializer.Serialize(entity));
            await sender.SendMessageAsync(message);
        }
    }

    public override async Task ReceiveMessage(string queueName, TEntity entity)
    {
        await using (var client = _ServiceBusClient)
        {
            ServiceBusReceiver receiver = client.CreateReceiver(queueName);
            ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
            if (receivedMessage != null)
            {
                Console.WriteLine($"Mensagem recebida: {receivedMessage.Body}");
                // Chama algum metodo para processar a mensagem, no final finaliza a mensagem!
                await receiver.CompleteMessageAsync(receivedMessage);
            }
        }
    }

    public override async Task ReceiveMessageAsync(string queueName, TEntity entity)
    {
        await using (var client = _ServiceBusClient)
        {
            var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false
            });

            processor.ProcessMessageAsync += async args =>
            {
                try
                {
                    var body = args.Message.Body.ToString();
                    Console.WriteLine($"Received: {body}");
                    await args.CompleteMessageAsync(args.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while processing the message. {ex}");
                }
            };

            processor.ProcessErrorAsync += async args =>
            {
                Console.WriteLine($"An error occurred while processing messages. {args.Exception}");
            };

            await processor.StartProcessingAsync();
            Console.WriteLine("Press any key to stop receiving messages...");
            Console.ReadKey();
            await processor.StopProcessingAsync();
        }

    }

    public override async Task SendMessage(string topicName, TEntity entity, CancellationToken cancellationToken = default)
    {
        var sender = _ServiceBusClient.CreateSender(topicName);

        var message = new ServiceBusMessage(JsonSerializer.Serialize(entity))
        {
            ContentType = "application/json",
            Subject = typeof(TEntity).Name
        };

        await sender.SendMessageAsync(message, cancellationToken);
    }

    public async Task ReceiveMessage(string topicName, string subscriptionName)
    {

        _processor = _ServiceBusClient.CreateProcessor(
            topicName,
            subscriptionName,
            new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 5
            });

        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _processor.StartProcessingAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await _processor.StopProcessingAsync(cancellationToken);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var body = args.Message.Body.ToString();
            var evento = JsonSerializer.Deserialize<dynamic>(body);
            await Task.Delay(1000);
            //Chama um metodo externo
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar: {ex.Message}");
            await args.DeadLetterMessageAsync(args.Message);
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        Console.WriteLine($"Erro no Service Bus: {args.Exception}");
        return Task.CompletedTask;
    }
}
