using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Options;

namespace WidgetCo.Store.Functions
{
    public abstract class BaseFunctionHandler
    {
        protected readonly ILogger _logger;
        protected readonly ApiOptions _apiOptions;

        protected BaseFunctionHandler(
            ILogger logger,
            IOptions<ApiOptions> apiOptions)
        {
            _logger = logger;
            _apiOptions = apiOptions.Value;
        }

        protected async Task<HttpResponseData> HandleException(
            HttpRequestData req,
            Exception ex,
            string operation)
        {
            if (ex is StoreException storeEx)
            {
                _logger.LogWarning(ex, "Store exception occurred during {Operation}", operation);
                return await CreateErrorResponse(
                    req,
                    storeEx.StatusCode,
                    storeEx.Message,
                    _apiOptions.ReturnDetailedErrors ?
                        storeEx.DetailedMessage ?? storeEx.OriginalException?.Message : null);
            }

            _logger.LogError(ex, "Unexpected error occurred during {Operation}", operation);
            return await CreateErrorResponse(
                req,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred",
                _apiOptions.ReturnDetailedErrors ? ex.Message : null);
        }

        protected static async Task<HttpResponseData> CreateErrorResponse(
            HttpRequestData req,
            int statusCode,
            string message,
            string? details = null)
        {
            var response = req.CreateResponse((HttpStatusCode)statusCode);
            await response.WriteAsJsonAsync(new { error = message, details });
            return response;
        }

        protected async Task<HttpResponseData> CreateSuccessResponse<T>(
            HttpRequestData req,
            HttpStatusCode statusCode,
            T data)
        {
            var response = req.CreateResponse(statusCode);
            await response.WriteAsJsonAsync(data);
            return response;
        }
    }
}