namespace WidgetCo.Store.Core.DTOs.Products
{
    public record UpdateProductRequest(
        string Id,
        string Name,
        decimal Price,
        string? Description,
        string ImageUrl
    );
}
