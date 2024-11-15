namespace WidgetCo.Store.Core.Interfaces.Storage
{
    public interface IImageRepository
    {
        Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType);
        Task<IEnumerable<string>> GetAllImageUrlsAsync();
        void ValidateImage(Stream imageStream, string fileName);
    }
}