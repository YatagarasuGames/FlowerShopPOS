using FlowerShop.Domain.Entities;
using FlowerShop.Infrastructure.Data;

namespace FlowerShop.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>
    {
        public ProductRepository(FlowersDbContext context) : base(context)
        {
        }

        public override void Delete(Product entity)
        {
            entity.IsActive = false;
        }
    }
}
