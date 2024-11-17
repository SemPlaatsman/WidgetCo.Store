using System.ComponentModel.DataAnnotations;

namespace WidgetCo.Store.Core.DTOs.Products
{
    public record CreateProductRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; init; } = default!;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; init; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; init; }

        [Required(ErrorMessage = "ImageUrl is required")]
        [Url(ErrorMessage = "ImageUrl must be a valid URL")]
        public string ImageUrl { get; init; } = default!;
    }
}
