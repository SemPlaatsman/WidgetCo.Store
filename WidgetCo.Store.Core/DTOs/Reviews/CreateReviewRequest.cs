using System.ComponentModel.DataAnnotations;

namespace WidgetCo.Store.Core.DTOs.Reviews
{
    public record CreateReviewRequest
    {
        [Required(ErrorMessage = "ProductId is required")]
        public string ProductId { get; init; } = default!;

        [Required(ErrorMessage = "ReviewText is required")]
        [StringLength(1000, ErrorMessage = "ReviewText cannot exceed 1000 characters")]
        public string ReviewText { get; init; } = default!;

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; init; }
    }
}
