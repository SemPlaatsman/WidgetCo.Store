using Microsoft.Extensions.Logging;
using System.Net;
using WidgetCo.Store.Core.DTOs.Reviews;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Queries;
using WidgetCo.Store.Infrastructure.Storage.Interfaces;

namespace WidgetCo.Store.Infrastructure.Handlers.Queries
{
    public class GetProductReviewsQueryHandler : IQueryHandler<GetProductReviewsQuery, IEnumerable<ReviewDto>>
    {
        private readonly IReviewRepository _repository;
        private readonly ILogger<GetProductReviewsQueryHandler> _logger;

        public GetProductReviewsQueryHandler(
            IReviewRepository repository,
            ILogger<GetProductReviewsQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<ReviewDto>> HandleAsync(GetProductReviewsQuery query)
        {
            try
            {
                var reviewEntities = await _repository.GetByProductIdAsync(query.ProductId);
                var reviews = reviewEntities.Select(e => e.ToDomain());

                return reviews.Select(r => new ReviewDto(
                    r.Id,
                    r.ProductId,
                    r.ReviewText,
                    r.Rating,
                    r.CreatedDate
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reviews for product {ProductId}", query.ProductId);
                throw new StoreException(
                    "Error retrieving reviews",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }
    }
}