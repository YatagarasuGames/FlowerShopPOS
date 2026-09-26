using FlowerShop.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace FlowerShop.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IRepository<Entities.Product> Products { get; }
        public  IRepository<Entities.Order> Orders { get; }
        public IRepository<Entities.OrderItem> OrderItems { get; }
        public IRepository<Entities.User> Users { get; }
        public IRepository<Entities.WriteOff> WriteOffs { get; }
        public IRepository<Entities.PriceHistory> PriceHistories { get; }
        IRepository<Supply> Supplies { get; }

        public Task<int> SaveChangesAsync();
        public Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
