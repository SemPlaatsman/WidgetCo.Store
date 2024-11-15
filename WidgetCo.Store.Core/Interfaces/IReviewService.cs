using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.DTOs.Reviews;
using WidgetCo.Store.Core.Queries;

namespace WidgetCo.Store.Core.Interfaces
{
    public interface IReviewService
    {
        Task<string> CreateReviewAsync(CreateReviewCommand command);
        Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(GetProductReviewsQuery query);
    }
}