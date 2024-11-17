// Storage related interfaces are kept seperate from the Core interfaces since they are a data concern and not a domain concern. 
using WidgetCo.Store.Infrastructure.Storage.Entities;

namespace WidgetCo.Store.Infrastructure.Storage.Interfaces
{
    public interface IReviewRepository
    {
        Task<string> CreateAsync(ReviewEntity review);
        Task<IEnumerable<ReviewEntity>> GetByProductIdAsync(string productId);
    }
}