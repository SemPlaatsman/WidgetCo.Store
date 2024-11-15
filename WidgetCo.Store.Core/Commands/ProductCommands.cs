namespace WidgetCo.Store.Core.Commands
{
    public record CreateProductCommand(
        string Name,
        decimal Price,
        string? Description,
        string ImageUrl
    );

    public record UpdateProductCommand(
        string Id,
        string Name,
        decimal Price,
        string? Description,
        string ImageUrl
    );
}
