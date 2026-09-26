using FlowerShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FlowerShop.Infrastructure.Data
{
    public class FlowersDbContext : DbContext
    {
        public FlowersDbContext(DbContextOptions<FlowersDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PriceHistory> PriceHistory { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WriteOff> WriteOffs { get; set; }
        public DbSet<Supply> Supplies { get; set; }
        public DbSet<ProductBatch> ProductBatches { get; set; }
        public DbSet<Composition> Compositions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.HasDefaultSchema("flowers_shop");
        }
    }
}
