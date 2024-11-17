using Microsoft.Extensions.Logging;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Infrastructure.Interfaces.Storage;
using WidgetCo.Store.Infrastructure.Util;

namespace WidgetCo.Store.Infrastructure.Services
{
    // CQRS does not apply to this service
    public class ProductImageService(
        IImageRepository imageRepository,
        ILogger<ProductImageService> logger) : IProductImageService
    {
        public Task<string> UploadImageAsync(Stream imageStream, string fileName) =>
            logger.ExecuteWithExceptionLoggingAsync(
                async () =>
                {
                    imageRepository.ValidateImage(imageStream, fileName);
                    var contentType = GetContentType(fileName);
                    return await imageRepository.UploadImageAsync(imageStream, fileName, contentType);
                },
                "Error uploading image {FileName}",
                fileName);

        public Task<IEnumerable<string>> GetAllImageUrlsAsync() =>
            logger.ExecuteWithExceptionLoggingAsync(
                () => imageRepository.GetAllImageUrlsAsync(),
                "Error retrieving all image URLs");

        private static string GetContentType(string fileName) =>
            Path.GetExtension(fileName).ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
    }
}