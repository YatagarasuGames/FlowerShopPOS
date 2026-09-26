using FlowerShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Domain.EntityConfigurations
{
    public class CompositionConfiguration : IEntityTypeConfiguration<Composition>
    {
        public void Configure(EntityTypeBuilder<Composition> builder)
        {
            builder.ToTable("compositions");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
            builder.Property(c => c.AssemblyPrice).IsRequired().HasDefaultValue(0f);
            builder.Property(c => c.TotalPrice).IsRequired();

            builder.HasOne(c => c.Order)
                .WithMany()
                .HasForeignKey(c => c.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}