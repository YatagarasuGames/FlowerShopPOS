using FlowerShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Domain.EntityConfigurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("order_items");

            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.Quantity)
                .IsRequired();

            builder.Property(oi => oi.PurchasePrice)
                .IsRequired();

            builder.Property(oi => oi.BaseUnitPrice)
                .IsRequired();

            builder.Property(oi => oi.FinalUnitPrice)
                .IsRequired();

            builder.Property(oi => oi.TotalPrice)
                .IsRequired();

            builder.Property(oi => oi.DiscountType)
                .IsRequired();

            builder.Property(oi => oi.DiscountValue)
                .IsRequired();

            builder.HasOne(oi => oi.Order)
                .WithMany()
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(oi => oi.ProductBatch)
                .WithMany()
                .HasForeignKey(oi => oi.ProductBatchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(oi => oi.Composition)
                .WithMany(c => c.Items)
                .HasForeignKey(oi => oi.CompositionId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        }
    }
}