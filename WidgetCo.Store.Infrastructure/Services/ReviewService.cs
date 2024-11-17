using Microsoft.Extensions.Logging;
using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.DTOs.Reviews;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Queries;
using WidgetCo.Store.Infrastructure.Util;

namespace WidgetCo.Store.Infrastructure.Services
{
    public class ReviewService(
        ICommandHandler<CreateReviewCommand, string> createReviewHandler,
        IQueryHandler<GetProductReviewsQuery, IEnumerable<ReviewDto>> getReviewsHandler,
        ILogger<ReviewService> logger) : IReviewService
    {
        public Task<string> CreateReviewAsync(CreateReviewCommand command) =>
            logger.ExecuteWithExceptionLoggingAsync(
                () => createReviewHandler.HandleAsync(command),
                "Error creating review for product {ProductId}",
                command.ProductId);

        public Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(GetProductReviewsQuery query) =>
            logger.ExecuteWithExceptionLoggingAsync(
                () => getReviewsHandler.HandleAsync(query),
                "Error retrieving reviews for product {ProductId}",
                query.ProductId);
    }
}