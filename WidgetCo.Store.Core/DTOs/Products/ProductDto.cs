namespace WidgetCo.Store.Core.DTOs.Products
{
    public record ProductDto(
        string Id,
        string Name,
        decimal Price,
        string? Description,
        string ImageUrl
    );
}