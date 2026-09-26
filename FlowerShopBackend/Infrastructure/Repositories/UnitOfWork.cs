using FlowerShop.Domain.Entities;
using FlowerShop.Domain.Interfaces;
using FlowerShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace FlowerShop.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FlowersDbContext _context;

        public IRepository<Product> Products { get; private set; }
        public IRepository<Order> Orders { get; private set; }
        public IRepository<OrderItem> OrderItems { get; private set; }
        public IRepository<User> Users { get; private set; }
        public IRepository<WriteOff> WriteOffs { get; private set; }
        public IRepository<PriceHistory> PriceHistories { get; private set; }
        public IRepository<Supply> Supplies { get; private set; }

        public UnitOfWork(FlowersDbContext context)
        {
            _context = context;

            Products = new ProductRepository(_context);
            Orders = new Repository<Order>(_context);
            OrderItems = new Repository<OrderItem>(_context);
            Users = new Repository<User>(_context);
            WriteOffs = new Repository<WriteOff>(_context);
            PriceHistories = new Repository<PriceHistory>(_context);
            Supplies = new Repository<Supply>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
