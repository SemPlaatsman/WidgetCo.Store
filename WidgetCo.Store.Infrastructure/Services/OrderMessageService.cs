using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Infrastructure.Options;
using WidgetCo.Store.Infrastructure.Util;

namespace WidgetCo.Store.Infrastructure.Services
{
    public class OrderMessageService(
       IOptions<OrderStorageOptions> options,
       ILogger<OrderMessageService> logger) : IOrderMessageService
    {
        private readonly QueueClient _queueClient = InitializeQueueClient(options.Value);

        public Task SendOrderProcessingMessageAsync(string orderProcessingMessage) =>
            logger.ExecuteWithExceptionLoggingAsync(
                () => _queueClient.SendMessageAsync(orderProcessingMessage),
                "Error sending order processing message {MessageContent}",
                orderProcessingMessage);

        private static QueueClient InitializeQueueClient(OrderStorageOptions options)
        {
            var client = new QueueClient(
                options.ConnectionString,
                options.QueueName,
                new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });

            client.CreateIfNotExists();
            return client;
        }
    }
}