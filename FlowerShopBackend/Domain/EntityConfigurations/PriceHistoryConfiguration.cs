using FlowerShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Domain.EntityConfigurations
{
    public class PriceHistoryConfiguration : IEntityTypeConfiguration<PriceHistory>
    {
        public void Configure(EntityTypeBuilder<PriceHistory> builder)
        {
            builder.ToTable("price_history");

            builder.HasKey(ph => ph.Id);

            builder.Property(ph => ph.OldPrice)
                .IsRequired();

            builder.Property(ph => ph.NewPrice)
                .IsRequired();

            builder.Property(ph => ph.ChangedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(ph => ph.Product)
                .WithMany()
                .HasForeignKey(ph => ph.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ph => ph.Cashier)
                .WithMany()
                .HasForeignKey(ph => ph.ChangedBy)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}