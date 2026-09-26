using FlowerShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Domain.EntityConfigurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.CurrentPrice)
                .IsRequired();

            builder.Property(p => p.Stock)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}