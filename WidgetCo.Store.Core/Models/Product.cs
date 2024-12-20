﻿namespace WidgetCo.Store.Core.Models
{
    public class Product
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = default!;
    }
}
