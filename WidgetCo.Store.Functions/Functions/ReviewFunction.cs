using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.DTOs.Reviews;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Options;
using WidgetCo.Store.Core.Queries;
using WidgetCo.Store.Functions;

public class ReviewFunctions : BaseFunctionHandler
{
    private readonly IReviewService _reviewService;

    public ReviewFunctions(
        IReviewService reviewService,
        ILogger<ReviewFunctions> logger,
        IOptions<ApiOptions> apiOptions)
        : base(logger, apiOptions)
    {
        _reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
    }

    [Function("AddReview")]
    public async Task<HttpResponseData> AddReview(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reviews")] HttpRequestData req)
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
            return await CreateSuccessResponse(
                req,
                HttpStatusCode.Created,
                new CreateReviewResponse(reviewId));
        }
        catch (Exception ex)
        {
            return await HandleException(req, ex, "adding review");
        }
    }

    [Function("GetProductReviews")]
    public async Task<HttpResponseData> GetProductReviews(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "reviews/{productId}")] HttpRequestData req,
        string productId)
    {
        try
        {
            var query = new GetProductReviewsQuery(productId);
            var reviews = await _reviewService.GetProductReviewsAsync(query);
            return await CreateSuccessResponse(req, HttpStatusCode.OK, reviews);
        }
        catch (Exception ex)
        {
            return await HandleException(req, ex, "retrieving reviews");
        }
    }
}