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
using WidgetCo.Store.Functions;

public class ProductImageFunctions : BaseFunctionHandler
{
    private readonly IProductImageService _imageService;

    public ProductImageFunctions(
        IProductImageService imageService,
        ILogger<ProductImageFunctions> logger,
        IOptions<ApiOptions> apiOptions)
        : base(logger, apiOptions)
    {
        _imageService = imageService;
    }

    [Function("UploadProductImage")]
    public async Task<HttpResponseData> UploadProductImage(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "images")] HttpRequestData req)
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

            return await CreateSuccessResponse(
                req,
                HttpStatusCode.Created,
                new UploadImageResponse(imageUrl));
        }
        catch (Exception ex)
        {
            return await HandleException(req, ex, "uploading image");
        }
    }

    [Function("GetProductImages")]
    public async Task<HttpResponseData> GetProductImages(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "images")] HttpRequestData req)
    {
        try
        {
            var imageUrls = await _imageService.GetAllImageUrlsAsync();
            return await CreateSuccessResponse(
                req,
                HttpStatusCode.OK,
                new ImageUrlsResponse(imageUrls));
        }
        catch (Exception ex)
        {
            return await HandleException(req, ex, "retrieving images");
        }
    }
}