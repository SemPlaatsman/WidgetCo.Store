namespace WidgetCo.Store.Infrastructure.Options
{
    public class OrderStorageOptions
    {
        public const string SectionName = "OrderStorage";
        public string ConnectionString { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
    }
}
