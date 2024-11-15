namespace WidgetCo.Store.Core.Commands
{
    public record CreateReviewCommand(
        string ProductId,
        string ReviewText,
        int Rating
    );
}
