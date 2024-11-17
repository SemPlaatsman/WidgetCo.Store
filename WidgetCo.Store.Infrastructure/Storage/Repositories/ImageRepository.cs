using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.RegularExpressions;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Options;
using WidgetCo.Store.Infrastructure.Interfaces.Storage;

namespace WidgetCo.Store.Infrastructure.Storage.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly BlobContainerClient _containerClient;
        private readonly ProductImageStorageOptions _options;
        private readonly ILogger<ImageRepository> _logger;

        public ImageRepository(
            IOptions<ProductImageStorageOptions> options,
            ILogger<ImageRepository> logger)
        {
            _options = options.Value;
            _logger = logger;

            _containerClient = new BlobContainerClient(
                _options.ConnectionString,
                _options.ContainerName);

            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType)
        {
            try
            {
                var blobName = $"{Guid.NewGuid()}-{SanitizeFileName(fileName)}";
                var blobClient = _containerClient.GetBlobClient(blobName);

                await blobClient.UploadAsync(imageStream, new BlobHttpHeaders
                {
                    ContentType = contentType
                });

                _logger.LogInformation("Successfully uploaded image: {BlobName}", blobName);
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image: {FileName}", fileName);
                throw new StoreException(
                    $"Error uploading image: {fileName}",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }

        public async Task<IEnumerable<string>> GetAllImageUrlsAsync()
        {
            try
            {
                var urls = new List<string>();
                await foreach (var blob in _containerClient.GetBlobsAsync())
                {
                    urls.Add($"{_containerClient.Uri}/{blob.Name}");
                }
                return urls;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving image URLs");
                throw new StoreException(
                    "Error retrieving image URLs",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }

        public void ValidateImage(Stream imageStream, string fileName)
        {
            if (imageStream.Length > _options.MaxFileSizeBytes)
            {
                throw new StoreException(
                    "File size exceeds maximum allowed size",
                    (int)HttpStatusCode.BadRequest,
                    $"Maximum allowed file size is {_options.MaxFileSizeBytes / 1024 / 1024}MB");
            }

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!_options.AllowedExtensions.Contains(extension))
            {
                throw new StoreException(
                    "Invalid file type",
                    (int)HttpStatusCode.BadRequest,
                    $"Allowed file types are: {string.Join(", ", _options.AllowedExtensions)}");
            }
        }

        private static string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var safeName = new string(fileName
                .Where(ch => !invalidChars.Contains(ch))
                .ToArray());

            return Regex.Replace(safeName, @"\s+", "-").ToLower();
        }
    }
}