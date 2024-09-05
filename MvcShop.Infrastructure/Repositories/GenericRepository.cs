using MvcShop.Infrastructure.Data;

namespace MvcShop.Infrastructure.Repositories
{
    public abstract class GenericRepository<T>(MvcShopContext context)
        : IRepository<T> where T : class
    {
        protected MvcShopContext _context = context;

        public virtual T Add(T entity)
        {
            var addedEntity = _context.Add(entity).Entity;
            return addedEntity;
        }

        public virtual T? Get(Guid id)
        {
            return _context.Find<T>(id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            var all = _context.Set<T>().ToList();
            return all;
        }

        public virtual void SaveChanges()
        {
            _context.SaveChanges();
        }

        public virtual T Update(T entity)
        {
            return _context.Update(entity).Entity;
        }
    }
}