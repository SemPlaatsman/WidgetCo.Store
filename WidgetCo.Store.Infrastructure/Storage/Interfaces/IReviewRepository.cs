using WidgetCo.Store.Infrastructure.Storage.Entities;

namespace WidgetCo.Store.Infrastructure.Storage.Interfaces
{
    public interface IReviewRepository
    {
        Task<string> CreateAsync(ReviewEntity review);
        Task<IEnumerable<ReviewEntity>> GetByProductIdAsync(string productId);
    }
}