using FlowerShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Domain.EntityConfigurations
{
    public class ProductBatchConfiguration : IEntityTypeConfiguration<ProductBatch>
    {
        public void Configure(EntityTypeBuilder<ProductBatch> builder)
        {
            builder.ToTable("product_batches");
            builder.HasKey(pb => pb.Id);

            builder.Property(pb => pb.PurchasePrice).IsRequired();
            builder.Property(pb => pb.SellingPrice).IsRequired();
            builder.Property(pb => pb.InitialQuantity).IsRequired();
            builder.Property(pb => pb.RemainingQuantity).IsRequired();
            builder.Property(pb => pb.ReceivedAt).IsRequired();

            builder.HasOne(pb => pb.Product)
                .WithMany()
                .HasForeignKey(pb => pb.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pb => pb.Supply)
                .WithMany()
                .HasForeignKey(pb => pb.SupplyId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(pb => new { pb.ProductId, pb.RemainingQuantity, pb.ReceivedAt });
        }
    }
}