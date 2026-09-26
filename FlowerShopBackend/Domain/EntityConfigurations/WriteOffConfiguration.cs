using FlowerShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Domain.EntityConfigurations
{
    public class WriteOffConfiguration : IEntityTypeConfiguration<WriteOff>
    {
        public void Configure(EntityTypeBuilder<WriteOff> builder)
        {
            builder.ToTable("write_offs");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Quantity)
                .IsRequired();

            builder.Property(w => w.Reason)
                .IsRequired();

            builder.Property(w => w.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(w => w.Product)
                .WithMany()
                .HasForeignKey(w => w.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(w => w.Cashier)
                .WithMany()
                .HasForeignKey(w => w.CashierId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}