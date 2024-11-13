using System.ComponentModel.DataAnnotations;

namespace WidgetCo.Store.Core.Models
{
    public class Review
    {
        [Required(ErrorMessage = "ProductId is required")]
        public required string ProductId { get; set; } = default!;

        [Required(ErrorMessage = "ReviewText is required")]
        [StringLength(1000, ErrorMessage = "ReviewText cannot exceed 1000 characters")]
        public required string ReviewText { get; set; } = default!;

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public required int Rating { get; set; }
    }
}
