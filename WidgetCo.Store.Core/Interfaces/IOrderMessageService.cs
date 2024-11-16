namespace WidgetCo.Store.Core.Interfaces
{
    public interface IOrderMessageService
    {
        Task SendOrderProcessingMessageAsync(string orderProcessingMessage);
    }
}
