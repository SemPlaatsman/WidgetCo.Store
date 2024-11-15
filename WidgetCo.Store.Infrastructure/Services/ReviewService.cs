using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data.Common;
using System.Net;
using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.DTOs.Reviews;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Extensions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Core.Queries;
using WidgetCo.Store.Infrastructure.Options;
using WidgetCo.Store.Infrastructure.Storage.Entities;

namespace WidgetCo.Store.Infrastructure.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ICommandHandler<CreateReviewCommand, string> _createReviewHandler;
        private readonly IQueryHandler<GetProductReviewsQuery, IEnumerable<ReviewDto>> _getReviewsHandler;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            ICommandHandler<CreateReviewCommand, string> createReviewHandler,
            IQueryHandler<GetProductReviewsQuery, IEnumerable<ReviewDto>> getReviewsHandler,
            ILogger<ReviewService> logger)
        {
            _createReviewHandler = createReviewHandler;
            _getReviewsHandler = getReviewsHandler;
            _logger = logger;
        }

        public async Task<string> CreateReviewAsync(CreateReviewCommand command)
        {
            return await _createReviewHandler.HandleAsync(command);
        }

        public async Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(GetProductReviewsQuery query)
        {
            return await _getReviewsHandler.HandleAsync(query);
        }
    }
}
