using System.Net;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WidgetCo.Store.Core.Options;
using Microsoft.Extensions.Options;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.DTOs.Reviews;
using WidgetCo.Store.Core.Queries;

namespace WidgetCo.Store.Functions.Functions
{
    public class ReviewFunction
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewFunction> _logger;
        private readonly ApiOptions _apiOptions;

        public ReviewFunction(
            IReviewService reviewService,
            ILogger<ReviewFunction> logger,
            IOptions<ApiOptions> apiOptions)
        {
            _reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiOptions = apiOptions.Value;
        }

        [Function("AddReview")]
        public async Task<IActionResult> AddReview(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reviews")] HttpRequest req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonSerializer.Deserialize<CreateReviewRequest>(
                    requestBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var command = new CreateReviewCommand(
                    request.ProductId,
                    request.ReviewText,
                    request.Rating
                );

                var reviewId = await _reviewService.CreateReviewAsync(command);
                return new CreatedResult(string.Empty, new CreateReviewResponse(reviewId));
            }
            catch (StoreException ex)
            {
                _logger.LogWarning(ex, "Store exception occurred while adding review");
                var error = new
                {
                    message = ex.Message,
                    details = _apiOptions.ReturnDetailedErrors ?
                        ex.DetailedMessage ?? ex.OriginalException?.Message : null
                };
                return new ObjectResult(error) { StatusCode = ex.StatusCode };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding review");
                return new ObjectResult(new
                {
                    message = "An unexpected error occurred",
                    details = _apiOptions.ReturnDetailedErrors ? ex.Message : null
                })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        [Function("GetProductReviews")]
        public async Task<IActionResult> GetProductReviews(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "reviews/{productId}")] HttpRequest req,
            string productId)
        {
            try
            {
                var query = new GetProductReviewsQuery(productId);
                var reviews = await _reviewService.GetProductReviewsAsync(query);
                return new OkObjectResult(reviews);
            }
            catch (StoreException ex)
            {
                _logger.LogWarning(ex, "Store exception occurred while retrieving reviews");
                var error = new
                {
                    message = ex.Message,
                    details = _apiOptions.ReturnDetailedErrors ?
                        ex.DetailedMessage ?? ex.OriginalException?.Message : null
                };
                return new ObjectResult(error) { StatusCode = ex.StatusCode };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving reviews");
                return new ObjectResult(new
                {
                    message = "An unexpected error occurred",
                    details = _apiOptions.ReturnDetailedErrors ? ex.Message : null
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
