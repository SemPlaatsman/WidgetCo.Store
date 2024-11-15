namespace WidgetCo.Store.Infrastructure.Options
{
    public class ReviewStorageOptions
    {
        public const string SectionName = "ReviewStorage";
        public string ConnectionString { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
    }
}
