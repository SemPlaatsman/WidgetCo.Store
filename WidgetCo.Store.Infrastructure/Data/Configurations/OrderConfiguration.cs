using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WidgetCo.Store.Core.Models;

namespace WidgetCo.Store.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrderRequestId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.CustomerId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.CreatedDate)
                .IsRequired();

            builder.Property(o => o.ShippedDate)
                .IsRequired(false);

            // Configure the Items collection
            builder.OwnsMany(o => o.Items, itemBuilder =>
            {
                itemBuilder.WithOwner().HasForeignKey("OrderId");
                itemBuilder.Property(i => i.ProductId).IsRequired();
                itemBuilder.Property(i => i.Quantity).IsRequired();
            });

            // Create indexes
            builder.HasIndex(o => o.OrderRequestId);
            builder.HasIndex(o => o.CustomerId);
        }
    }
}
