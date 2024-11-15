using Microsoft.Extensions.Logging;
using System.Net;
using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Extensions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Infrastructure.Storage.Entities;
using WidgetCo.Store.Infrastructure.Storage.Interfaces;

namespace WidgetCo.Store.Infrastructure.Handlers
{
    public class CreateReviewCommandHandler : ICommandHandler<CreateReviewCommand, string>
    {
        private readonly IReviewRepository _repository;
        private readonly ILogger<CreateReviewCommandHandler> _logger;

        public CreateReviewCommandHandler(
            IReviewRepository repository,
            ILogger<CreateReviewCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<string> HandleAsync(CreateReviewCommand command)
        {
            try
            {
                var review = new Review
                {
                    ProductId = command.ProductId,
                    ReviewText = command.ReviewText,
                    Rating = command.Rating,
                    CreatedDate = DateTime.UtcNow
                };

                // Domain validation
                review.ValidateAndThrow();

                var reviewEntity = ReviewEntity.FromDomain(review);
                return await _repository.CreateAsync(reviewEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review for product {ProductId}", command.ProductId);
                throw new StoreException(
                    "Error creating review",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }
    }
}