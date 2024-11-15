namespace WidgetCo.Store.Core.DTOs.Products
{
    public record CreateProductRequest(
        string Name,
        decimal Price,
        string? Description,
        string ImageUrl
    );
}
