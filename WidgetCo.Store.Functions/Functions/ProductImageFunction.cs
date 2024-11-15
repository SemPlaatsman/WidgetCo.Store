using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using WidgetCo.Store.Core.DTOs.Images;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Options;

namespace WidgetCo.Store.Functions
{
    public class ProductImageFunctions
    {
        private readonly IProductImageService _imageService;
        private readonly ILogger<ProductImageFunctions> _logger;
        private readonly ApiOptions _apiOptions;

        public ProductImageFunctions(
            IProductImageService imageService,
            ILogger<ProductImageFunctions> logger,
            IOptions<ApiOptions> apiOptions)
        {
            _imageService = imageService;
            _logger = logger;
            _apiOptions = apiOptions.Value;
        }

        [Function("UploadProductImage")]
        public async Task<HttpResponseData> UploadProductImage(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "images")] HttpRequestData req)
        {
            try
            {
                var httpContext = req.FunctionContext.GetHttpContext()
                    ?? throw new InvalidOperationException("HttpContext not available");

                var form = await httpContext.Request.ReadFormAsync();
                var file = form.Files.FirstOrDefault();

                if (file == null)
                {
                    throw new StoreException(
                        "No file found in request",
                        StatusCodes.Status400BadRequest);
                }

                using var stream = file.OpenReadStream();
                var imageUrl = await _imageService.UploadImageAsync(stream, file.FileName);

                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(new UploadImageResponse(imageUrl));
                return response;
            }
            catch (StoreException ex)
            {
                _logger.LogWarning(ex, "Store exception occurred while uploading image");
                return await CreateErrorResponse(req, ex.StatusCode, ex.Message,
                    _apiOptions.ReturnDetailedErrors ? ex.DetailedMessage : null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while uploading image");
                return await CreateErrorResponse(req,
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred",
                    _apiOptions.ReturnDetailedErrors ? ex.Message : null);
            }
        }

        [Function("GetProductImages")]
        public async Task<HttpResponseData> GetProductImages(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "images")] HttpRequestData req)
        {
            try
            {
                var imageUrls = await _imageService.GetAllImageUrlsAsync();
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new ImageUrlsResponse(imageUrls));
                return response;
            }
            catch (StoreException ex)
            {
                _logger.LogWarning(ex, "Store exception occurred while retrieving images");
                return await CreateErrorResponse(req, ex.StatusCode, ex.Message,
                    _apiOptions.ReturnDetailedErrors ? ex.DetailedMessage : null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product images");
                return await CreateErrorResponse(req, (int)HttpStatusCode.InternalServerError,
                    "Error retrieving images", _apiOptions.ReturnDetailedErrors ? ex.Message : null);
            }
        }

        private static async Task<HttpResponseData> CreateErrorResponse(
            HttpRequestData req,
            int statusCode,
            string message,
            string? details = null)
        {
            var response = req.CreateResponse((HttpStatusCode)statusCode);
            await response.WriteAsJsonAsync(new { error = message, details });
            return response;
        }
    }
}