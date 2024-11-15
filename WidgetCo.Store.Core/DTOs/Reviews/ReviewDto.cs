namespace WidgetCo.Store.Core.DTOs.Reviews
{
    public record ReviewDto(
        string Id,
        string ProductId,
        string ReviewText,
        int Rating,
        DateTime CreatedDate
    );
}
