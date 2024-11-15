namespace WidgetCo.Store.Core.Options
{
    public class ProductImageStorageOptions
    {
        public const string SectionName = "BlobStorage";
        public string ConnectionString { get; set; } = string.Empty;
        public string ContainerName { get; set; } = string.Empty;
        public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024; // 10MB default
    }
}