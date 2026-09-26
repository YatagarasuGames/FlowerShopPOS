using FlowerShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Domain.EntityConfigurations
{
    public class SupplyConfiguration : IEntityTypeConfiguration<Supply>
    {
        public void Configure(EntityTypeBuilder<Supply> builder)
        {
            builder.ToTable("supplies");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Quantity)
                .IsRequired();

            builder.Property(s => s.PurchasePrice)
                .IsRequired();

            builder.Property(s => s.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(s => s.Product)
                .WithMany()
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.ReceivedBy)
                .WithMany()
                .HasForeignKey(s => s.ReceivedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}