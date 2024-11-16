using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Infrastructure.Options;

namespace WidgetCo.Store.Infrastructure.Services
{
    public class OrderMessageService : IOrderMessageService
    {
        private readonly QueueClient _queueClient;

        public OrderMessageService(IOptions<OrderStorageOptions> options)
        {
            _queueClient = new QueueClient(options.Value.ConnectionString, options.Value.QueueName, new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });
            _queueClient.CreateIfNotExists();
        }

        public async Task SendOrderProcessingMessageAsync(string orderProcessingMessage)
        {
            await _queueClient.SendMessageAsync(orderProcessingMessage);
        }
    }
}
