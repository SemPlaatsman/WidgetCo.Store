// Storage related interfaces are kept seperate from the Core interfaces since they are a data concern and not a domain concern. 
namespace WidgetCo.Store.Infrastructure.Interfaces.Storage
{
    public interface IImageRepository
    {
        Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType);
        Task<IEnumerable<string>> GetAllImageUrlsAsync();
        void ValidateImage(Stream imageStream, string fileName);
    }
}