using Microsoft.Extensions.Logging;
using WidgetCo.Store.Core.Interfaces.Storage;
using WidgetCo.Store.Core.Interfaces;

namespace WidgetCo.Store.Infrastructure.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly ILogger<ProductImageService> _logger;

        public ProductImageService(
            IImageRepository imageRepository,
            ILogger<ProductImageService> logger)
        {
            _imageRepository = imageRepository;
            _logger = logger;
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
        {
            _imageRepository.ValidateImage(imageStream, fileName);

            var contentType = GetContentType(fileName);
            return await _imageRepository.UploadImageAsync(imageStream, fileName, contentType);
        }

        public async Task<IEnumerable<string>> GetAllImageUrlsAsync()
        {
            return await _imageRepository.GetAllImageUrlsAsync();
        }

        private static string GetContentType(string fileName)
        {
            return Path.GetExtension(fileName).ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}