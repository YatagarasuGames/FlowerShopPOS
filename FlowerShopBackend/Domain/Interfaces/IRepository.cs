using System.Linq.Expressions;

namespace FlowerShop.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        public Task<T?> GetByIdAsync(Guid id);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        public Task AddAsync(T entity);
        public Task AddRangeAsync(IEnumerable<T> entities);
        public void Update(T entity);
        public void Delete(T entity);
    }
}
