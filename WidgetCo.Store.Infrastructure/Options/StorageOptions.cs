namespace WidgetCo.Store.Infrastructure.Options
{
    public class StorageOptions
    {
        public const string SectionName = "AzureStorage";
        public string ConnectionString { get; set; } = string.Empty;
    }
}
