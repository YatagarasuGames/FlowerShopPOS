using FlowerShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Domain.EntityConfigurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.TotalAmount)
                .IsRequired();

            builder.Property(o => o.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("now()");

            builder.HasOne(o => o.Cashier)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.CashierId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
