using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Options;

namespace WidgetCo.Store.Api.Controllers
{
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly ILogger _logger;
        protected readonly ApiOptions _apiOptions;

        protected BaseApiController(ILogger logger, IOptions<ApiOptions> apiOptions)
        {
            _logger = logger;
            _apiOptions = apiOptions.Value;
        }

        protected IActionResult HandleException(Exception ex, string operation)
        {
            if (ex is StoreException storeEx)
            {
                _logger.LogWarning(ex, "Store exception occurred during {Operation}", operation);
                var error = new
                {
                    message = storeEx.Message,
                    details = _apiOptions.ReturnDetailedErrors ?
                        storeEx.DetailedMessage ?? storeEx.OriginalException?.Message : null
                };
                return StatusCode(storeEx.StatusCode, error);
            }

            _logger.LogError(ex, "Unexpected error occurred during {Operation}", operation);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                new { message = "An unexpected error occurred" });
        }
    }
}
