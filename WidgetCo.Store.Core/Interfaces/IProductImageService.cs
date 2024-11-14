namespace WidgetCo.Store.Core.Interfaces
{
    public interface IProductImageService
    {
        Task<string> UploadImageAsync(Stream imageStream, string fileName);

        Task<IEnumerable<string>> GetAllImageUrlsAsync();
    }
}