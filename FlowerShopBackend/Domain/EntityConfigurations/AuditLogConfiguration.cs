using FlowerShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Domain.EntityConfigurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("audit_logs");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Username).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Action).IsRequired().HasMaxLength(100);
            builder.Property(a => a.EntityName).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Details).IsRequired();
            builder.Property(a => a.CreatedAt).IsRequired();

            builder.HasIndex(a => a.CreatedAt);
            builder.HasIndex(a => a.UserId);
        }
    }
}