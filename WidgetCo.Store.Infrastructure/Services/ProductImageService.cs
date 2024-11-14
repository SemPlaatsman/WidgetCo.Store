using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.RegularExpressions;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Options;

namespace WidgetCo.Store.Infrastructure.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly ILogger<ProductImageService> _logger;
        private readonly BlobStorageOptions _options;

        public ProductImageService(
            IOptions<BlobStorageOptions> options,
            ILogger<ProductImageService> logger)
        {
            _options = options.Value;
            _containerClient = new BlobContainerClient(
                _options.ConnectionString,
                _options.ContainerName);

            // Create the container if it doesn't exist
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);

            _logger = logger;
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
        {
            try
            {
                ValidateImage(imageStream, fileName);

                // Sanitize and make filename unique
                var blobName = $"{Guid.NewGuid()}-{SanitizeFileName(fileName)}";
                var blobClient = _containerClient.GetBlobClient(blobName);

                await blobClient.UploadAsync(imageStream, new BlobHttpHeaders
                {
                    ContentType = GetContentType(fileName)
                });

                _logger.LogInformation("Successfully uploaded image: {BlobName}", blobName);
                return blobClient.Uri.ToString();
            }
            catch (Exception ex) when (ex is not StoreException)
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

        private void ValidateImage(Stream imageStream, string fileName)
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
            // Remove invalid characters
            var invalidChars = Path.GetInvalidFileNameChars();
            var safeName = new string(fileName
                .Where(ch => !invalidChars.Contains(ch))
                .ToArray());

            // Replace spaces with dashes and convert to lowercase
            return Regex.Replace(safeName, @"\s+", "-").ToLower();
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