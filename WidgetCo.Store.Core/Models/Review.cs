namespace WidgetCo.Store.Core.Models
{
    public class Review
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ProductId { get; set; } = default!;
        public string ReviewText { get; set; } = default!;
        public int Rating { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
