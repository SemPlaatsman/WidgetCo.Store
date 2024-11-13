using WidgetCo.Store.Core.Models;

namespace WidgetCo.Store.Core.Interfaces
{
    public interface IReviewService
    {
        Task AddReviewAsync(Review review);
        Task<IEnumerable<Review>> GetProductReviewsAsync(string productId);
    }
}
